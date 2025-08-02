using Verse;

namespace ReSpliceCharmweavers;

public class Gene_Thrall : Gene
{
    public override void ExposeData()
    {
        base.ExposeData();
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            if (!pawn.IsLovethrall())
                pawn.genes.RemoveGene(this);
        }
    }
}