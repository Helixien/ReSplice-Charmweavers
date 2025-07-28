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
            if (!HandleHediffForTemperatureInsensitive(___pawn, ref hediff))
            {
                return false;
            }
            if (!HandleHediffForMultiPregnancy(___pawn, ref hediff))
            {
                return false;
            }
            return true;
        }

        public static bool HandleHediffForTemperatureInsensitive(Pawn ___pawn, ref Hediff hediff)
        {
            if (hediff.def == HediffDefOf.Hypothermia || hediff.def == HediffDefOf.Heatstroke)
            {
                return !___pawn.HasActiveGene(RS_DefOf.RS_TemperatureInsensitive);
            }

            return true;
        }

        public static bool HandleHediffForMultiPregnancy(Pawn ___pawn, ref Hediff hediff)
        {
            if (hediff.def == HediffDefOf.PregnantHuman)
            {
                return !___pawn.HasActiveGene(RS_DefOf.RS_MultiPregnancy) || !___pawn.health.hediffSet.HasHediff(RS_DefOf.RS_RecentImpregnation);
            }
            return true;
        }
    }
}
