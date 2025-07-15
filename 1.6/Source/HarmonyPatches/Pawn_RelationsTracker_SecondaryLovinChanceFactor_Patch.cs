using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor")]
    public static class Pawn_RelationsTracker_SecondaryLovinChanceFactor_Patch
    {
        public static void Postfix(ref float __result, Pawn_RelationsTracker __instance, Pawn otherPawn)
        {
            if (__instance.pawn.IsLovethrall(out var master) && otherPawn == master)
            {
                __result = Mathf.Max(3, __result);
            }
        }
    }
}
