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
        public static void Prefix(Pawn geneticMother)
        {
            mother = geneticMother;
        }

        public static void Postfix()
        {
            mother = null;
        }
    }
}
