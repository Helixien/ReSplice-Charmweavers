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
            if (other.def == HediffDefOf.PregnantHuman && __instance.def == HediffDefOf.PregnantHuman && __instance.pawn.HasActiveGene(RS_DefOf.RS_MultiPregnancy))
            {
                return false;
            }
            return true;
        }
    }
}
