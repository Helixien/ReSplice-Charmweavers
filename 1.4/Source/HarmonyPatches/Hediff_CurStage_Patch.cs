using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Hediff), "CurStage", MethodType.Getter)]
    public static class Hediff_CurStage_Patch
    {
        public static HashSet<HediffStage> clonedStages = new HashSet<HediffStage>();
        public static void Postfix(Hediff __instance, ref HediffStage __result)
        {
            if (__instance is Hediff_Pregnant pregnant && __instance.pawn.HasGene(RS_DefOf.RS_MultiPregnancy))
            {
                var newStage = __result.Clone();
                var capMods = __result.capMods.ToList();
                newStage.capMods = new List<PawnCapacityModifier>();
                foreach (var mod in capMods)
                {
                    newStage.capMods.Add(new PawnCapacityModifier
                    {
                        capacity = mod.capacity,
                        offset = mod.offset * 0.5f,
                    });
                }
                newStage.hungerRateFactorOffset *= 0.5f;
                __result = newStage;
            }
        }
    }
}
