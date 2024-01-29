using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryRomanceChanceFactor")]
    public static class Pawn_RelationsTracker_SecondaryRomanceChanceFactor_Patch
    {
        public static void Postfix(ref float __result, Pawn_RelationsTracker __instance, Pawn otherPawn)
        {
            if (__instance.pawn.HasGene(RS_DefOf.RS_Beauty_Angelic))
            {
                __result *= 1.6f;
            }
        }
    }
}
