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
        var master = pawn.GetMaster();
        if (!CaravanExitMapUtility.CanExitMapAndJoinOrCreateCaravanNow(pawn))
            return null;
        if (!(master.IsFormingCaravan() || master.CurJob?.exitMapOnArrival is true))
        {
            return null;
        }
        return base.TryGiveJob(pawn);
    }
}