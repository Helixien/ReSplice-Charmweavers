using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ReSpliceCharmweavers
{
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
}
