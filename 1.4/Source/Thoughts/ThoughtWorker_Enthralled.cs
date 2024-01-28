using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Enthralled : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            return other.IsLovethrall(out var master) && p != master;
        }
    }
}
