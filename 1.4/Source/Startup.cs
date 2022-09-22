using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace RareXenotypesSuccubus
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            new Harmony("RareXenotypesSuccubus.Mod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(PregnancyUtility), "ApplyBirthOutcome")]
    public static class PregnancyUtility_ApplyBirthOutcome_Patch
    {
        public static Pawn mother;
        public static void Prefix(OutcomeChance outcome, float quality, Precept_Ritual ritual, List<GeneDef> genes,
            Pawn geneticMother, Thing birtherThing, Pawn father = null, Pawn doctor = null, LordJob_Ritual lordJobRitual = null, RitualRoleAssignments assignments = null)
        {
            mother = geneticMother;
        }

        public static void Postfix()
        {
            mother = null;
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        public static void Prefix(ref PawnGenerationRequest request)
        {
            if (PregnancyUtility_ApplyBirthOutcome_Patch.mother != null)
            {
                var gene = PregnancyUtility_ApplyBirthOutcome_Patch.mother.genes.GetGene(RX_DefOf.RX_Matrianic);
                if (gene != null && Rand.Chance(0.9f))
                {
                    request.FixedGender = Gender.Female;
                }
                PregnancyUtility_ApplyBirthOutcome_Patch.mother = null;
            }
        }
    }
}
