using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(Recipe_ExtractOvum), nameof(Recipe_ExtractOvum.OnSurgerySuccess))]
public static class Recipe_ExtractOvum_OnSurgerySuccess_Patch
{
    private static void Postfix(Pawn pawn)
    {
        if (pawn.genes?.HasActiveGene(RS_DefOf.RS_MultiPregnancy) == true)
        {
            var disappearsComp = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf.OvumExtracted)?.TryGetComp<HediffComp_Disappears>();
            if (disappearsComp != null)
                disappearsComp.ticksToDisappear = GenDate.TicksPerDay;
        }
    }
}