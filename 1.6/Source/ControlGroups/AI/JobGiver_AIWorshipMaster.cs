using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobGiver_AIWorshipMaster : ThinkNode_JobGiver
{
    private const float MaxWorshipDistance = 10f;

    public override Job TryGiveJob(Pawn pawn)
    {
        // Roughly based on JobGiver_Nuzzle

        if (pawn.WorkTagIsDisabled(WorkTags.Social))
            return null;
        if (!pawn.IsLovethrall(out var master))
            return null;
        if (!pawn.Position.InHorDistOf(master.Position, MaxWorshipDistance))
            return null;
        if (pawn.GetRoom() != master.GetRoom())
            return null;
        if (!master.CanCasuallyInteractNow())
            return null;

        var job = JobMaker.MakeJob(RS_DefOf.RS_Worship, master);
        job.locomotionUrgency = LocomotionUrgency.Walk;
        job.expiryInterval = 3000;
        return job;
    }
}