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
                var thrallAmount = p.GetThrallAmount();
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
    }
}
