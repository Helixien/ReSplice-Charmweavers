using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Hediff), "CurStage", MethodType.Getter)]
    public static class Hediff_CurStage_Patch
    {
        public static Dictionary<HediffStage, HediffStage> adjustedStages = [];

        public static void Postfix(Hediff __instance, ref HediffStage __result)
        {
            if (__instance is Hediff_Pregnant && __instance.pawn.HasActiveGene(RS_DefOf.RS_MultiPregnancy))
            {
                if (!adjustedStages.TryGetValue(__result, out var newStage))
                {
                    newStage = __result.Clone();
                    newStage.capMods = [];
                    if (__result.capMods is not null)
                    {
                        foreach (var mod in __result.capMods)
                        {
                            var copy = mod.Clone();
                            copy.offset *= 0.5f;
                            newStage.capMods.Add(copy);
                        }
                    }
                    newStage.hungerRateFactorOffset *= 0.5f;
                    adjustedStages[__result] = newStage;
                }
                __result = newStage;
            }
        }
    }
}
