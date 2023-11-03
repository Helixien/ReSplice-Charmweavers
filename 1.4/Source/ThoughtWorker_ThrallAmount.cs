using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_ThrallAmount : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.genes?.HasGene(RS_DefOf.RS_PsychicEnthralling) ?? false)
            {
                var thrallAmount = GetAllThralls(p).Count;
                if (thrallAmount <= 6)
                {
                    return ThoughtState.ActiveAtStage(thrallAmount);
                }
                else
                {
                    return ThoughtState.ActiveAtStage(6);
                }
            }
            return ThoughtState.Inactive;
        }

        public List<Pawn> GetAllThralls(Pawn p)
        {
            return p.relations.DirectRelations.Where(x => x.def == RS_DefOf.RX_Thrall).Select(x => x.otherPawn).ToList();
        }
    }
}
