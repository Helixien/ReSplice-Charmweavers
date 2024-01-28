using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Precept_LovehexerPresent : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive || p.IsLovehexer())
            {
                return ThoughtState.Inactive;
            }
            foreach (Pawn item in p.MapHeld.mapPawns.AllPawnsSpawned)
            {
                if (item.IsLovehexer() && (item.IsPrisonerOfColony || item.IsSlaveOfColony || item.IsColonist))
                {
                    return ThoughtState.ActiveDefault;
                }
            }
            return ThoughtState.Inactive;
        }
    }
}
