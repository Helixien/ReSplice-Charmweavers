using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn), "Destroy")]
    public static class Pawn_Destroy_Patch
    {
        public static void Postfix(Pawn __instance)
        {
            if (__instance.Destroyed)
            {
                Pawn_Kill_Patch.RemoveThrallRelationships(__instance);
            }
        }
    }
}
