using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_BondedWithMyMaster : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.IsLovethrall())
            {
                return ThoughtState.ActiveDefault;
            }
            return ThoughtState.Inactive;
        }
    }
}
