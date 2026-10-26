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
            var mother = PregnancyUtility_ApplyBirthOutcome_Patch.mother;
            var father = PregnancyUtility_ApplyBirthOutcome_Patch.father;
            var carrier = mother?.gender == Gender.Female && mother.HasActiveGene(RS_DefOf.RS_Matrianic) ? mother
                : father?.gender == Gender.Male && father.HasActiveGene(RS_DefOf.RS_Matrianic) ? father : null;
            if (carrier is not null && Rand.Chance(0.9f))
            {
                request.FixedGender = carrier.gender;
            }
            PregnancyUtility_ApplyBirthOutcome_Patch.mother = null;
            PregnancyUtility_ApplyBirthOutcome_Patch.father = null;
        }
    }
}
