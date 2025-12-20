using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Enthralled : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            // Not a thrall, don't care
            if (!other.IsLovethrall(out var master))
                return false;
            // My thrall, don't care
            if (p == master)
                return false;
            // If this pawn is not a thrall or shares master with the other one, care
            return !p.IsLovethrall(out var otherMaster) || master != otherMaster;
        }
    }
}
