using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(InteractionWorker_MarriageProposal), "RandomSelectionWeight")]
    public static class InteractionWorker_MarriageProposal_RandomSelectionWeight_Patch
    {
        public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.HasPrecept(RS_DefOf.RS_Marriage_Abhorrent) || recipient.HasPrecept(RS_DefOf.RS_Marriage_Abhorrent))
            {
                __result = 0f;
            }
            else if (initiator.gender == Gender.Male && initiator.HasPrecept(RS_DefOf.RS_Marriage_FemaleOnly))
            {
                __result = 0f;
            }
            else if (initiator.gender == Gender.Female && initiator.HasPrecept(RS_DefOf.RS_Marriage_MaleOnly))
            {
                __result = 0f;
            }
        }
    }
}
