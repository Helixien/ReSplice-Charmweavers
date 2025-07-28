using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(HediffSet), "AddDirect")]
    public static class HediffSet_AddDirect_Patch
    {
        [HarmonyPriority(int.MaxValue)]
        private static bool Prefix(HediffSet __instance, Pawn ___pawn, Hediff hediff)
        {
            if (!Pawn_HealthTracker_AddHediff_Patch.HandleHediffForTemperatureInsensitive(___pawn, ref hediff))
            {
                return false;
            }
            if (!Pawn_HealthTracker_AddHediff_Patch.HandleHediffForMultiPregnancy(___pawn, ref hediff))
            {
                return false;
            }
            return true;
        }

        private static void Postfix(HediffSet __instance, Pawn ___pawn, Hediff hediff)
        {
            if (hediff.def == HediffDefOf.PregnantHuman && ___pawn.HasActiveGene(RS_DefOf.RS_MultiPregnancy) && __instance.hediffs.Contains(hediff))
            {
                ___pawn.health.AddHediff(RS_DefOf.RS_RecentImpregnation);
            }
        }
    }
}
