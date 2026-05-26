using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class MentalState_Enthraller : MentalState
{
    private const int NoLongerValidTargetCheckInterval = 120;

    public Pawn target;

    public override void PreStart()
    {
        base.PreStart();
        TryFindNewTarget();
    }

    public override void MentalStateTick(int delta)
    {
        base.MentalStateTick(delta);

        // Recover if we enthralled our target
        if (target != null && target.GetMaster() == pawn)
            RecoverFromState();

        if (pawn.IsHashIntervalTick(NoLongerValidTargetCheckInterval, delta) && !IsTargetStillValidAndReachable())
        {
            var ability = pawn.abilities.GetAbility(RS_DefOf.RS_PsychicEnthralling);
            if (ability == null || !ability.CanCast || !TryFindNewTarget())
                RecoverFromState();
            else
                Messages.Message("RS.MessageEnthrallerChangedTarget".Translate(pawn.NameShortColored, target.Label, pawn.Named("PAWN"), target.Named("TARGET")).Resolve().AdjustedFor(pawn), pawn, MessageTypeDefOf.NegativeEvent);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_References.Look(ref target, nameof(target));
    }

    public override TaggedString GetBeginLetterText()
    {
        if (target == null)
        {
            Log.Error("No target. This should have been checked in this mental state's worker.");
            return string.Empty;
        }

        return def.beginLetter.Formatted(pawn.NameShortColored, target.NameShortColored, pawn.Named("PAWN"), target.Named("TARGET")).AdjustedFor(pawn).Resolve().CapitalizeFirst();
    }

    private bool TryFindNewTarget()
    {
        target = Core.FindPawnToEnthrall(pawn);
        return target != null;
    }

    public bool IsTargetStillValidAndReachable()
    {
        var spawnedTarget = target?.SpawnedParentOrMe;
        // Make sure target and spawned parent aren't null and on the same map as this pawn
        if (spawnedTarget == null || spawnedTarget.Map != pawn.Map)
            return false;
        // Make sure the target isn't dead
        if (target.Dead)
            return false;
        // Make sure the spawned parent is the same as target
        if (spawnedTarget is Pawn && spawnedTarget != target)
            return false;
        // Make sure we can reach
        if (!pawn.CanReach(spawnedTarget, PathEndMode.Touch, Danger.Deadly))
            return false;
        // Don't enthrall thralls/lovehexers
        if (target.IsLovehexer() || target.IsLovethrall())
            return false;

        return true;
    }
}