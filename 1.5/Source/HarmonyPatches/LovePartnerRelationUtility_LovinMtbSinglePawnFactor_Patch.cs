using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(LovePartnerRelationUtility), "LovinMtbSinglePawnFactor")]
    public static class LovePartnerRelationUtility_LovinMtbSinglePawnFactor_Patch
    {
        public static void Postfix(ref float __result, Pawn pawn)
        {
            if (pawn.IsLovethrall())
            {
                __result = Mathf.Max(2, __result);
            }
        }
    }
}
