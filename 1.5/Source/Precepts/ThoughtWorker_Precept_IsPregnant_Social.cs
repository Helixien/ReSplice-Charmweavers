using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Precept_IsPregnant_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            return PregnancyUtility.GetPregnancyHediff(otherPawn) != null;
        }
    }
}
