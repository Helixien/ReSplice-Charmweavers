using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Hediff), "TryMergeWith")]
    public static class Hediff_TryMergeWith_Patch
    {
        public static bool Prefix(Hediff __instance, Hediff other)
        {
            if (other.def == HediffDefOf.PregnantHuman && __instance.def == HediffDefOf.PregnantHuman)
            {
                if (__instance.pawn.HasActiveGene(RS_DefOf.RS_MultiPregnancy))
                {
                    if (__instance.pawn.health.hediffSet.hediffs.Count(x => x.def == HediffDefOf.PregnantHuman) >= ReSpliceCharmweaversSettings.maxMultiPregnancyAmount)
                    {
                        return true;
                    }
                    __instance.pawn.health.AddHediff(RS_DefOf.RS_RecentImpregnation);
                    return false;
                }
            }
            return true;
        }
    }
}
