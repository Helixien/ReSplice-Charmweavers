using RimWorld;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    public class PawnRelationWorker_CoSpouse : PawnRelationWorker
    {
        public override bool InRelation(Pawn me, Pawn other)
        {
            if (me == other || other.relations is null || me.Dead || other.Dead)
            {
                return false;
            }
            if (LovePartnerRelationUtility.LovePartnerRelationExists(me, other))
            {
                return false;
            }
            var spouses = me.relations.DirectRelations.Where(x => x.def == PawnRelationDefOf.Spouse).Select(x => x.otherPawn).ToList();
            if (spouses.Contains(other) is false)
            {
                var otherSpouses = other.relations.DirectRelations.Where(x => x.def == PawnRelationDefOf.Spouse).Select(x => x.otherPawn).ToList();
                if (spouses.Intersect(otherSpouses).Any())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
