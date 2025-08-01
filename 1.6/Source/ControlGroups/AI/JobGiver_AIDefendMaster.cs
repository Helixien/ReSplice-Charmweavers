using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobGiver_AIDefendMaster : JobGiver_AIDefendPawn
{
    public override Pawn GetDefendee(Pawn pawn) => pawn.GetMaster();

    public override float GetFlagRadius(Pawn pawn) => 5f;

    public override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.playerSettings == null || pawn.WorkTagIsDisabled(WorkTags.Violent))
            return base.TryGiveJob(pawn);

        // Force a pawn to fight when defending their master, rather than fleeing/ignoring enemies
        var response = pawn.playerSettings.hostilityResponse;
        try
        {
            pawn.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
            return base.TryGiveJob(pawn);
        }
        finally
        {
            pawn.playerSettings.hostilityResponse = response;
        }
    }
}