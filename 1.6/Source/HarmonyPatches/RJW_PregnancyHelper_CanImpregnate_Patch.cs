using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch("rjw.PregnancyHelper", "CanImpregnate")]
public static class RJW_PregnancyHelper_CanImpregnate_Patch
{
    private static FastInvokeHandler UseVanillaPregnancyGetter;

    private static bool Prepare(MethodBase method)
    {
        if (method != null)
            return true;
        if (!ModsConfig.IsActive("rim.job.world"))
            return false;

        var getter = AccessTools.DeclaredPropertyGetter("rjw.RJWPregnancySettings:UseVanillaPregnancy");
        if (getter == null)
        {
            Log.Error("RJW compat failed, cannot check if vanilla pregnancy is enabled. Multi-pregnancy won't work.");
            return false;
        }
        UseVanillaPregnancyGetter = MethodInvoker.GetHandler(getter);

        return true;
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
    {
        var target = AccessTools.DeclaredMethod("rjw.PawnExtensions:IsPregnant");
        var addition = typeof(RJW_PregnancyHelper_CanImpregnate_Patch).DeclaredMethod(nameof(IsNormalPregnancy));

        foreach (var ci in instr)
        {
            yield return ci;

            if (ci.Calls(target))
            {
                // Load the second argument (potential mother pawn)
                yield return CodeInstruction.LoadArgument(1);
                // Call our method to check if the mother can currently support multi pregnancy
                yield return new CodeInstruction(OpCodes.Call, addition);
                // Bitwise & the original result and our method results
                yield return new CodeInstruction(OpCodes.And);
            }
        }
    }

    private static bool IsNormalPregnancy(Pawn potentialMother)
    {
        // If using RJW pregnancy system, don't allow multi-pregnancy
        if (!(bool)UseVanillaPregnancyGetter(null))
            return true;

        // Some sanity checks
        if (potentialMother.genes == null || potentialMother.health?.hediffSet == null)
            return true;

        // If the pawn doesn't have the multi pregnancy gene - normal pregnancy
        if (!potentialMother.genes.HasActiveGene(RS_DefOf.RS_MultiPregnancy))
            return true;

        // If the pawn has a recent impregnation, can't get impregnated again - normal pregnancy
        if (potentialMother.health.hediffSet.HasHediff(RS_DefOf.RS_RecentImpregnation))
            return true;

        // If the pawn doesn't have a pregnancy already, they can't get a multi-pregnancy.
        // If at this point the pawn is considered pregnant, it's most likely a RJW pregnancy (like a mechanoid one).
        if (!potentialMother.health.hediffSet.HasHediff(HediffDefOf.PregnantHuman))
            return true;

        // We went through all other checks, so we can allow a multi-pregnancy
        return false;
    }
}