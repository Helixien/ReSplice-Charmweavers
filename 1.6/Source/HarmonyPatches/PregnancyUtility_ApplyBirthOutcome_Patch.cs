using HarmonyLib;
using RimWorld;
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

        public static void Postfix(Thing birtherThing)
        {
            // Recent pregnancy for ideo. 3 day cooldown before pawns are upset about not being pregnant.
            if (RS_DefOf.RS_RecentPregnancy != null && birtherThing is Pawn birtherPawn)
            {
                birtherPawn.health.AddHediff(RS_DefOf.RS_RecentPregnancy);
            }
        }

        public static void Finalizer()
        {
            mother = null;
        }
    }
}
