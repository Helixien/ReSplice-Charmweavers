using HarmonyLib;
using System;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        public static void Prefix(ref PawnGenerationRequest request)
        {
            if (PregnancyUtility_ApplyBirthOutcome_Patch.mother != null)
            {
                var gene = PregnancyUtility_ApplyBirthOutcome_Patch.mother.genes.GetGene(RS_DefOf.RS_Matrianic);
                if (gene != null && Rand.Chance(0.9f))
                {
                    request.FixedGender = Gender.Female;
                }
                PregnancyUtility_ApplyBirthOutcome_Patch.mother = null;
            }
        }
    }
}
