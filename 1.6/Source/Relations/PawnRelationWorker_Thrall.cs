using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class PawnRelationWorker_Thrall : PawnRelationWorker
    {
        public override void OnRelationRemoved(Pawn firstPawn, Pawn secondPawn)
        {
            base.OnRelationRemoved(firstPawn, secondPawn);
            var rel = secondPawn.relations.GetDirectRelation(RS_DefOf.RS_Master, firstPawn);
            if (rel != null && PawnRelationWorker_Master.recursionTrap is false)
            {
                try
                {
                    PawnRelationWorker_Master.recursionTrap = true;
                    secondPawn.relations.RemoveDirectRelation(rel);
                }
                finally
                {
                    PawnRelationWorker_Master.recursionTrap = false;
                }
            }
            var hediff = firstPawn.health.hediffSet.GetFirstHediffOfDef(RS_DefOf.RS_LoveThrall) as Hediff_LoveThrall;
            if (hediff != null && !hediff.recursionTrap)
            {
                hediff.pawn.health.RemoveHediff(hediff);
            }
        }
    }
}
