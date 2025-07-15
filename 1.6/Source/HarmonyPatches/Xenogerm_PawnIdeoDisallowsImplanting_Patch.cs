using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Xenogerm), "PawnIdeoDisallowsImplanting")]
    public static class Xenogerm_PawnIdeoDisallowsImplanting_Patch
    {
        public static void Postfix(Xenogerm __instance, Pawn selPawn, ref bool __result)
        {
            if (!IdeoUtility.DoerWillingToDo(RS_DefOf.RS_PropagateLovehexerGene, selPawn) && __instance.GeneSet.GenesListForReading.Any((GeneDef x) => x == RS_DefOf.RS_Lovehexer))
            {
                __result = true;
            }
        }
    }
}
