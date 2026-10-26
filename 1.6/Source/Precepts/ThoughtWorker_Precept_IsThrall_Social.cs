using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Precept_IsThrall_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            if (!otherPawn.IsLovethrall(out var master) || master == null || p == master)
            {
                return false;
            }

            // Thralls sharing a master do not judge each other for being enthralled.
            return !p.IsLovethrall(out var otherMaster) || master != otherMaster;
        }
    }
}
