using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(CompAbilityEffect_ReimplantXenogerm), "PawnIdeoCanAcceptReimplant")]
    public static class CompAbilityEffect_ReimplantXenogerm_PawnIdeoCanAcceptReimplant_Patch
    {
        public static void Postfix(CompAbilityEffect_ReimplantXenogerm __instance, Pawn implanter, Pawn implantee, ref bool __result)
        {
            if (!IdeoUtility.DoerWillingToDo(RS_DefOf.RS_PropagateLovehexerGene, implantee) && implanter.genes.Xenogenes.Any((Gene x) => x.def == RS_DefOf.RS_PsychicEnthralling))
            {
                __result = false;
            }
        }
    }
}
