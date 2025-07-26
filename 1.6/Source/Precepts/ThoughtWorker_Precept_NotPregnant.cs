using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Precept_NotPregnant : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            return p.gender == Gender.Female && PregnancyUtility.GetPregnancyHediff(p) is null && !p.health.hediffSet.HasHediff(RS_DefOf.RS_RecentPregnancy);
        }
    }
}
