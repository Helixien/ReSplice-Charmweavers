using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff", new Type[]
    {
        typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult)
    })]
    public static class Pawn_HealthTracker_AddHediff_Patch
    {
        [HarmonyPriority(int.MaxValue)]
        private static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, ref Hediff hediff, BodyPartRecord part = null, DamageInfo? dinfo = null, DamageWorker.DamageResult result = null)
        {
            if (___pawn.HasGene(RS_DefOf.RS_TemperatureInsensitive))
            {
                return HandleHediffForTemperatureInsensitive(___pawn, ref hediff);
            }
            return true;
        }

        public static bool HandleHediffForTemperatureInsensitive(Pawn ___pawn, ref Hediff hediff)
        {
            if (hediff.def == HediffDefOf.Hypothermia)
            {
                return false;
            }
            else if (hediff.def == HediffDefOf.Heatstroke)
            {
                return false;
            }
            return true;
        }
    }
}
