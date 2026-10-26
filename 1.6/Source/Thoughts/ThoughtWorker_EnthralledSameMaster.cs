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
        // Active if this pawn is a thrall under the same master
        return p.IsLovethrall(out var otherMaster) && master == otherMaster;
    }
}