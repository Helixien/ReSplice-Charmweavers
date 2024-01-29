using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(HediffSet), "HasHediffPreventsPregnancy")]
    public static class HediffSet_HasHediffPreventsPregnancy_Patch
    {
        public static bool Prefix(HediffSet __instance, ref bool __result)
        {
            if (__instance.pawn.HasGene(RS_DefOf.RS_MultiPregnancy))
            {
                __result = HasHediffPreventsPregnancyForMultiPregnancy(__instance);
                return false;
            }
            return true;
        }

        public static bool HasHediffPreventsPregnancyForMultiPregnancy(HediffSet __instance)
        {
            for (int i = 0; i < __instance.hediffs.Count; i++)
            {
                if (__instance.hediffs[i].def.preventsPregnancy && __instance.hediffs[i] is not HediffWithParents)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
