using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Hediff_Pregnant), "GestationProgress", MethodType.Getter)]
    public static class Hediff_Pregnant_GestationProgress_Patch
    {
        public static void Postfix(Hediff_Pregnant __instance, ref float __result)
        {
            if (__instance.Severity >= 1 && __instance.pawn.health.hediffSet.InLabor(false))
            {
                __instance.Severity = __result = 0.999f;
            }
        }
    }
}
