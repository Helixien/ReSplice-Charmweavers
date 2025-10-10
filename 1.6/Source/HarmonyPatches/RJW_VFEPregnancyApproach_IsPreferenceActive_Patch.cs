using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch("rjw.Modules.Interactions.Preferences.TryForBabyPreferences", "IsActive")]
[HarmonyPatchCategory(RJW_VFEPregnancyApproach_GetPreference_Patch.RJWTryForBaby_VFEPregnancyApproach_Patches)]
public static class RJW_VFEPregnancyApproach_IsPreferenceActive_Patch
{
    private static FastInvokeHandler CanImpregnateMethod;

    private static bool Prepare(MethodBase method)
    {
        if (method != null)
            return true;
        if (!ModsConfig.IsActive("rim.job.world"))
            return false;

        try
        {
            var canImpregnateMethod = AccessTools.DeclaredMethod("rjw.PregnancyHelper:CanImpregnate");
            if (canImpregnateMethod == null)
            {
                Error("Failed to initialize RJW compat, could not find `CanImpregnate` method.");
                return false;
            }
            CanImpregnateMethod = MethodInvoker.GetHandler(canImpregnateMethod);
        }
        catch (Exception e)
        {
            Error($"Failed to initialize RJW compat, exception encountered:\n{e}");
        }
        
        return true;

        void Error(string error)
        {
            // Cleanup
            CanImpregnateMethod = null;
            // Log an error
            Log.Error(error);
        }
    }

    private static bool Prefix(Pawn pawn, Pawn partner, out bool __result)
    {
        if (pawn.relations == null || partner.relations == null)
        {
            __result = false;
            return false;
        }

        var interactionType = RJW_VFEPregnancyApproach_GetPreference_Patch.IntToInteractionType(1);
        if (!(bool)CanImpregnateMethod(null, pawn, partner, interactionType) && !(bool)CanImpregnateMethod(null, partner, pawn, interactionType))
        {
            __result = false;
            return false;
        }

        var data = pawn.relations.GetAdditionalPregnancyApproachData();
        if (data?.partners != null && data.partners.TryGetValue(partner, out var def) && (def.pregnancyChanceFactorBase.HasValue || def.pregnancyChanceForPartners.HasValue))
            __result = true;
        else if (pawn.relations.GetPregnancyApproachForPartner(partner) != PregnancyApproach.Normal)
            __result = true;
        else
            __result = false;

        return false;
    }
}