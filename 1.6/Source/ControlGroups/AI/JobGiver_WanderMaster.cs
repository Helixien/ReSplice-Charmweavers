using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobGiver_WanderMaster : JobGiver_Wander
{
    public JobGiver_WanderMaster()
    {
        wanderRadius = 3;
        ticksBetweenWandersRange = new IntRange(125, 200);
    }

    private GlobalTargetInfo Target(Pawn pawn) => pawn.GetMaster();

    public override Job TryGiveJob(Pawn pawn)
    {
        var target = Target(pawn);
        if (target.Map != pawn.Map)
            return null;

        var job = base.TryGiveJob(pawn);
        job.reportStringOverride = reportStringOverride.Formatted(target.Thing.Named("TARGET"));
        return job;
    }

    public override IntVec3 GetWanderRoot(Pawn pawn) => Target(pawn).Cell;

    public override void DecorateGotoJob(Job job)
    {
        job.expiryInterval = 120;
        expireOnNearbyEnemy = true;
    }
}