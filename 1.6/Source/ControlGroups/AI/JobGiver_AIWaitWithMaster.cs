using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobGiver_AIWaitWithMaster : ThinkNode_JobGiver
{
    private const float RandomCellNearRadius = 1.9f;

    public override Job TryGiveJob(Pawn pawn)
    {
        if (!pawn.IsLovethrall(out var master) || master.Awake())
            return null;

        var cell = CanUseCell(pawn.Position, pawn) ? pawn.Position : GetWaitDest(master.Position, pawn);
        if (!cell.IsValid)
            return null;

        var job = JobMaker.MakeJob(JobDefOf.Wait_WithSleeping, cell, master);
        job.expiryInterval = 120;
        job.expireOnEnemiesNearby = true;
        return job;
    }

    private IntVec3 GetWaitDest(IntVec3 root, Pawn thrall)
    {
        if (CellFinder.TryFindRandomReachableNearbyCell(root, thrall.Map, RandomCellNearRadius, TraverseParms.For(thrall), c => CanUseCell(c, thrall), null, out var cell))
            return cell;
        return IntVec3.Invalid;
    }

    private bool CanUseCell(IntVec3 c, Pawn thrall)
    {
        var map = thrall.Map;
        return c.Standable(map) && thrall.CanReach(c, PathEndMode.OnCell, Danger.Deadly) && thrall.CanReserve(c) && c.GetDoor(map) == null;
    }
}