using HarmonyLib;
using RimWorld;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "Notify_PawnSold")]
    public static class Pawn_RelationsTracker_Notify_PawnSold_Patch
    {
        public static void Postfix(Pawn_RelationsTracker __instance)
        {
            Pawn_Kill_Patch.FreeAllThrals(__instance.pawn);
        }
    }
}
