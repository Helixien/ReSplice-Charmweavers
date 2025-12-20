using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

public class ThoughtWorker_EnthralledSameMaster : ThoughtWorker
{
    public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
    {
        // Not a thrall, don't care
        if (!other.IsLovethrall(out var master))
            return false;
        // My thrall, don't care
        if (p == master)
            return false;
        // If this pawn is a trhall under same master, active
        return p.IsLovethrall(out var otherMaster) && master == otherMaster;
    }
}