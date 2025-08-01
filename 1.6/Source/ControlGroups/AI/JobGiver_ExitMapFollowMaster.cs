using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobGiver_ExitMapFollowMaster : JobGiver_ExitMapBest
{
    public JobGiver_ExitMapFollowMaster()
    {
        failIfCantJoinOrCreateCaravan = true;
    }

    public override Job TryGiveJob(Pawn pawn)
    {
        if (!CaravanExitMapUtility.CanExitMapAndJoinOrCreateCaravanNow(pawn))
            return null;
        return base.TryGiveJob(pawn);
    }
}