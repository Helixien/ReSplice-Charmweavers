using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(LovePartnerRelationUtility), "GetLovinMtbHours")]
    public static class LovePartnerRelationUtility_GetLovinMtbHours_Patch
    {
        public static void Postfix(ref float __result, Pawn pawn, Pawn partner)
        {
            if ((pawn.HasGene(RS_DefOf.RS_Libido_None)) 
                || (partner.HasGene(RS_DefOf.RS_Libido_None)))
            {
                __result = -1f;
            }
        }
    }
}
