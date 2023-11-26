using HarmonyLib;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(PawnNameColorUtility), "PawnNameColorOf")]
    public static class PawnNameColorUtility_PawnNameColorOf_Patch
    {
        public static void Postfix(ref Color __result, Pawn pawn)
        {
            if (pawn.IsLoveThrall())
            {
                __result = Core.ThrallColor;
            }
        }
    }
}
