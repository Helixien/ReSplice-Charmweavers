using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class PawnRelationWorker_Master : PawnRelationWorker
    {
        public static bool recursionTrap;
        public override void OnRelationRemoved(Pawn firstPawn, Pawn secondPawn)
        {
            base.OnRelationRemoved(firstPawn, secondPawn);
            var rel = secondPawn.relations.GetDirectRelation(RS_DefOf.RS_Thrall, firstPawn);
            if (rel != null && recursionTrap is false)
            {
                try
                {
                    recursionTrap = true;
                    secondPawn.relations.RemoveDirectRelation(rel);
                }
                finally
                {
                    recursionTrap = false;
                }
            }
            var hediff = secondPawn.health.hediffSet.GetFirstHediffOfDef(RS_DefOf.RS_LoveThrall) as Hediff_LoveThrall;
            if (hediff != null && !hediff.recursionTrap)
            {
                hediff.pawn.health.RemoveHediff(hediff);
            }
        }
    }
}
