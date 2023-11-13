using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "TryGiveThoughts")]
    public static class PawnDiedOrDownedThoughtsUtility_TryGiveThoughts_Patch
    {
        public static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind)
        {
            if (thoughtsKind == PawnDiedOrDownedThoughtsKind.Lost 
                || thoughtsKind == PawnDiedOrDownedThoughtsKind.Died
                || thoughtsKind == PawnDiedOrDownedThoughtsKind.BanishedToDie)
            {
                Pawn_Kill_Patch.FreeAllThrals(victim);
            }
        }
    }
}
