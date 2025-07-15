using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(JobGiver_DoLovin), "TryGiveJob")]
    public static class JobGiver_DoLovin_TryGiveJob_Patch
    {
        public static void Postfix(ref Job __result, Pawn pawn)
        {
            if (pawn.HasActiveGene(RS_DefOf.RS_Libido_None))
            {
                __result = null;
            }
            if (__result != null)
            {
                var target = __result.targetA.Pawn;
                if (target != null && (target.HasActiveGene(RS_DefOf.RS_Libido_None)))
                {
                    __result = null;
                }
            }
        }
    }
}
