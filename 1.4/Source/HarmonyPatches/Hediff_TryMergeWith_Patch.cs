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
                if (__instance.pawn.HasGene(RS_DefOf.RS_MultiPregnancy))
                {
                    if (__instance.pawn.health.hediffSet.hediffs.Count(x => x.def == HediffDefOf.PregnantHuman) >= 3)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
