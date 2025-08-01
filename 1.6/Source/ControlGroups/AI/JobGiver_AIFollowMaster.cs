using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobGiver_AIFollowMaster : JobGiver_AIFollowPawn
{
    public override int FollowJobExpireInterval => 200;

    public override Pawn GetFollowee(Pawn pawn) => pawn.GetMaster();

    public override float GetRadius(Pawn pawn) => 5f;

    public override Job TryGiveJob(Pawn pawn)
    {
        if (!pawn.IsLovethrall())
            return null;
        return base.TryGiveJob(pawn);
    }
}