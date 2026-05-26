using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobGiver_Enthraller : ThinkNode_JobGiver
{
    public override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.MentalState is not MentalState_Enthraller state || !state.IsTargetStillValidAndReachable())
            return null;

        var ability = pawn.abilities.GetAbility(RS_DefOf.RS_PsychicEnthralling);
        if (ability == null || !ability.CanCast)
            return null;

        if (state.target.IsPsychologicallyInvisible())
            return null;

        if (ability.verb.CanHitTarget(state.target))
            return ability.GetJob(state.target, state.target);

        var job = JobMaker.MakeJob(JobDefOf.Goto, state.target);
        job.locomotionUrgency = LocomotionUrgency.Sprint;
        job.intervalScalingTarget = TargetIndex.A;
        job.checkOverrideOnExpire = true;
        job.collideWithPawns = true;
        return job;
    }
}