using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(InteractionWorker_MarriageProposal), "AcceptanceChance")]
    public static class InteractionWorker_MarriageProposal_AcceptanceChance_Patch
    {
        public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (recipient.HasPrecept(RS_DefOf.RS_Marriage_Abhorrent))
            {
                __result = 0f;
            }
        }
    }
}
