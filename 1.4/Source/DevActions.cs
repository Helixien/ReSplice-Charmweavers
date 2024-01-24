using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    public static class DevActions
    {
        [DebugAction("Pawns", "Force breakup", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ForceBreakup(Pawn p)
        {
            foreach (var otherPawn in p.relations.DirectRelations
                .Where(x => LovePartnerRelationUtility.IsLovePartnerRelation(x.def)).Select(x => x.otherPawn).ToList())
            {
                RS_DefOf.Breakup.Worker.Interacted(p, otherPawn, new List<RulePackDef>(), out var letterText, out var letterLabel, out var letterDef, out var lookTargets);
                if (letterDef != null)
                {
                    Find.LetterStack.ReceiveLetter(letterLabel, letterText, letterDef, lookTargets ?? ((LookTargets)p));
                }
            }
        }
    }
}
