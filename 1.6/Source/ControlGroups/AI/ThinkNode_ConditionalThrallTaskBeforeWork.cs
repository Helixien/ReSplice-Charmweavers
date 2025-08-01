using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class ThinkNode_ConditionalThrallTaskBeforeWork : ThinkNode_Conditional
{
    // Used inside a node that already checks for colonists
    public override bool Satisfied(Pawn pawn) => pawn.IsLovethrall();

    public override float GetPriority(Pawn pawn)
    {
        // JobGiver_Work has 0
        if (pawn.workSettings is not { EverWork: true })
            return 2.001f;

        // Always have 0.01 higher priority than work
        var assignment = pawn.timetable == null ? TimeAssignmentDefOf.Anything : pawn.timetable.CurrentAssignment;
        if (assignment == TimeAssignmentDefOf.Anything)
            return 5.501f;
        if (assignment == TimeAssignmentDefOf.Work)
            return 9.001f;
        if (assignment == TimeAssignmentDefOf.Sleep)
            return 3.001f;

        // JobGiver_Work gives 2 when assignment is joy/meditate, and throws if any other assignment.
        return 2.001f;
    }
}