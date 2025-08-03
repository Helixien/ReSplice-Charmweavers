using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Precept_NotPregnant_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            return Find.Storyteller.difficulty.ChildrenAllowed && otherPawn.gender == Gender.Female
                   && PregnancyUtility.GetPregnancyHediff(otherPawn) is null && !otherPawn.health.hediffSet.HasHediff(RS_DefOf.RS_RecentPregnancy);
        }
    }
}
