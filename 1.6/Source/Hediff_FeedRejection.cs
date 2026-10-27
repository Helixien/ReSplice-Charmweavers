using System.Linq;
using Verse;

namespace ReSpliceCharmweavers;

public class Hediff_FeedRejection : HediffWithComps
{
    public Pawn target;

    public override bool TryMergeWith(Hediff other) => false;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref target, "target");
    }

    public static bool WasRejected(Pawn biter, Pawn target) => biter.health.hediffSet.hediffs.Any(x => x is Hediff_FeedRejection h && h.target == target);

    public static void NotifyRejected(Pawn biter, Pawn target)
    {
        var hediff = HediffMaker.MakeHediff(RS_DefOf.RS_FeedRejection, biter) as Hediff_FeedRejection;
        hediff.target = target;
        biter.health.AddHediff(hediff);
    }
}
