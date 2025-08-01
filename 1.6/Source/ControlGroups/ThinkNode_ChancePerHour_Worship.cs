using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class ThinkNode_ChancePerHour_Worship : ThinkNode_ChancePerHour
{
    public override float MtbHours(Pawn pawn)
    {
        // No job
        if (pawn.skills == null || pawn.WorkTagIsDisabled(WorkTags.Social))
            return 0f;

        // 1.5 at level 0, 0.5 at level 20.
        return 1.5f - 0.05f * pawn.skills.GetSkill(SkillDefOf.Social).Level;
    }
}