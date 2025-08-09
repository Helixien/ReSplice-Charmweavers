using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class ThinkNode_ConditionalThrallWorkMode : ThinkNode_Conditional
{
    public ThrallWorkModeDef workMode;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var copy = (ThinkNode_ConditionalThrallWorkMode)base.DeepCopy(resolve);
        copy.workMode = workMode;
        return copy;
    }

    public override bool Satisfied(Pawn pawn)
    {
        var controlGroup = pawn.GetThrallControlGroup();
        return controlGroup != null && controlGroup.WorkMode == workMode && (workMode.allowWhileMasterAsleep || controlGroup.Gene.pawn.Awake());
    }
}