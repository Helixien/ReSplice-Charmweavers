using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class MentalBreakWorker_Enthraller : MentalBreakWorker
{
    public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
    {
        var mult = 1f;
        var gene = pawn.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>();
        if (gene == null)
        {
            mult = 0;
        }
        else
        {
            var thralls = pawn.GetThrallAmount();
            if (thralls >= gene.LovethrallCapacity)
                mult = 0f;
            else if (thralls == 0)
                mult = 3f;
        }

        return base.CommonalityFor(pawn, moodCaused) * mult;
    }
}