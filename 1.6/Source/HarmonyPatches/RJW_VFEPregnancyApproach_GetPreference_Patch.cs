using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch("rjw.Modules.Interactions.Preferences.TryForBabyPreferences", "GetPreferences")]
[HarmonyPatchCategory(RJWTryForBaby_VFEPregnancyApproach_Patches)]
public static class RJW_VFEPregnancyApproach_GetPreference_Patch
{
    public const string RJWTryForBaby_VFEPregnancyApproach_Patches = "RJWTryForBaby_VFEPregnancyApproach_Patches";

    private static Type RjwInteractionTypeEnumType;
    private static Type TypePreferenceType;
    private static Type ListOfPreferencesType;

    private static bool Prepare(MethodBase method)
    {
        if (method != null)
            return true;
        if (!ModsConfig.IsActive("rim.job.world"))
            return false;

        try
        {
            RjwInteractionTypeEnumType = AccessTools.TypeByName("rjw.xxx").Inner("rjwSextype");
            if (RjwInteractionTypeEnumType == null)
            {
                Error("Failed to initialize RJW compat, could not find interaction type.");
                return false;
            }

            TypePreferenceType = AccessTools.TypeByName("rjw.Modules.Interactions.Preferences.TypePreference");
            if (TypePreferenceType == null)
            {
                Error("Failed to initialize RJW compat, could not find interaction preferences.");
                return false;
            }

            var type = AccessTools.TypeByName("rjw.Modules.Interactions.Preferences.SexPreference");
            if (type == null)
            {
                Error("Failed to initialize RJW compat, could not find interaction preferences.");
                return false;
            }
            ListOfPreferencesType = typeof(List<>).MakeGenericType(type);
        }
        catch (Exception e)
        {
            Error($"Failed to initialize RJW compat, exception encountered:\n{e}");
        }

        return true;

        void Error(string error)
        {
            // Cleanup
            ListOfPreferencesType = RjwInteractionTypeEnumType = TypePreferenceType = null;
            // Log an error
            Log.Error(error);
        }
    }

    private static bool Prefix(Pawn pawn, Pawn partner, out IEnumerable __result)
    {
        var list = (IList)Activator.CreateInstance(ListOfPreferencesType);
        __result = list;

        if (pawn.relations == null || partner.relations == null)
            return false;

        var data = pawn.relations.GetAdditionalPregnancyApproachData();
        if (data?.partners != null && data.partners.TryGetValue(partner, out var def))
        {
            // Pregnancy chance for partners is a static
            if (def.pregnancyChanceForPartners.HasValue)
                list.Add(Activator.CreateInstance(TypePreferenceType, IntToInteractionType(1), def.pregnancyChanceForPartners.Value));
            // Pregnancy chance factor base multiplies the base value of pregnancy chance further.
            // Use that value to increase/decrease the chance of a specific interaction happening.
            if (def.pregnancyChanceFactorBase.HasValue)
                list.Add(Activator.CreateInstance(TypePreferenceType, IntToInteractionType(1), def.pregnancyChanceFactorBase.Value));

            return false;
        }

        var approach = pawn.relations.GetPregnancyApproachForPartner(partner);

        // When trying for baby, multiply the weight of a compatible interaction by 4.
        // The mod uses the value of 10, but let's use the exact percentages the interaction does for sake of consistency with VFE approaches.
        if (approach == PregnancyApproach.TryForBaby)
            list.Add(Activator.CreateInstance(TypePreferenceType, IntToInteractionType(1), 4f));
        // When avoiding pregnancy, multiply the weight of a compatible interaction by 0.25.
        // This will make this interaction less likely to happen, thus reducing pregnancy change. RWJ itself at the moment doesn't actually support this approach.
        else if (approach == PregnancyApproach.AvoidPregnancy)
            list.Add(Activator.CreateInstance(TypePreferenceType, IntToInteractionType(1), 0.25f));

        return false;
    }

    public static object IntToInteractionType(int interactionType) => Enum.ToObject(RjwInteractionTypeEnumType, interactionType);
}