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
            if (p.HasActiveGene(RS_DefOf.RS_Lovehexer))
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
