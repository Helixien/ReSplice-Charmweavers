using RimWorld.Planet;

namespace ReSpliceCharmweavers;

public class ThrallWorkModeDrawer_FollowMaster : ThrallWorkModeDrawer
{
    protected override bool DrawIconAtTarget => false;

    public override GlobalTargetInfo GetTargetForLine(ThrallControlGroup group) => group.Target.Pawn;
}