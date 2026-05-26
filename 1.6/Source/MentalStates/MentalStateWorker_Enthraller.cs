using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class MentalStateWorker_Enthraller : MentalStateWorker
{
    public override bool StateCanOccur(Pawn pawn)
    {
        if (!base.StateCanOccur(pawn))
            return false;

        var gene = pawn.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>();
        if (gene == null)
            return false;

        var thralls = pawn.GetThrallAmount();
        if (thralls >= gene.LovethrallCapacity)
            return false;

        var ability = pawn.abilities.GetAbility(RS_DefOf.RS_PsychicEnthralling);
        if (ability == null || !ability.CanCast)
            return false;

        if (Core.FindPawnToEnthrall(pawn) == null)
            return false;

        return true;
    }
}