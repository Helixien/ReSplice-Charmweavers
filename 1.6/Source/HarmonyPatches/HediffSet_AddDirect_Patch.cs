using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(HediffSet), "AddDirect")]
    public static class HediffSet_AddDirect_Patch
    {
        [HarmonyPriority(int.MaxValue)]
        private static bool Prefix(HediffSet __instance, Pawn ___pawn, Hediff hediff)
        {
            if (___pawn.HasActiveGene(RS_DefOf.RS_TemperatureInsensitive))
            {
                return Pawn_HealthTracker_AddHediff_Patch.HandleHediffForTemperatureInsensitive(___pawn, ref hediff);
            }
            return true;
        }
    }
}
