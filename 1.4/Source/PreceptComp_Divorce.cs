using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ReSpliceCharmweavers
{
    public class PreceptComp_Divorce : PreceptComp_Thought
    {
        public ThoughtDef thoughtSocial;
        public void DoDivorceThought(Pawn member, List<Pawn> otherPawns, Precept precept)
        {
            if (member.needs?.mood != null)
            {
                foreach (var otherPawn in otherPawns)
                {
                    Thought_Memory thought_Memory = ThoughtMaker.MakeThought(thought, precept);
                    member.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, otherPawn);
                    Thought_MemorySocial thought_Social = ThoughtMaker.MakeThought(thoughtSocial) as Thought_MemorySocial;
                    member.needs.mood.thoughts.memories.TryGainMemory(thought_Social, otherPawn);
                }
            }
        }
    }
}
