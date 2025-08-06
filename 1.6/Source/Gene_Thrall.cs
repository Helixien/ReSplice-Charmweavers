using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

public class Gene_Thrall : Gene
{
    public override void PostRemove()
    {
        base.PostRemove();

        // 10% chance to keep changes to sexuality after no longer lovethralled
        if (pawn.story?.traits != null && Rand.Chance(0.1f))
        {
            // If the gene was suppressing any of the sexuality traits, remove those (unless added by traits)
            if (def.suppressedTraits != null)
            {
                foreach (var trait in def.suppressedTraits)
                {
                    if (trait.def == TraitDefOf.Bisexual || trait.def == TraitDefOf.Gay || trait.def == TraitDefOf.Asexual)
                    {
                        var current = pawn.story.traits.GetTrait(trait.def);
                        // Only remove if not forced by other genes
                        if (current is { sourceGene: null })
                            pawn.story.traits.RemoveTrait(current);
                    }
                }
            }

            // If the gene was forcing any of the sexuality traits, add those (unless there's already any)
            if (def.forcedTraits != null && !pawn.story.traits.HasTrait(TraitDefOf.Bisexual) && !pawn.story.traits.HasTrait(TraitDefOf.Gay) && !pawn.story.traits.HasTrait(TraitDefOf.Asexual))
            {
                foreach (var trait in def.forcedTraits)
                {
                    if (trait.def == TraitDefOf.Bisexual || trait.def == TraitDefOf.Gay || trait.def == TraitDefOf.Asexual)
                        pawn.story.traits.GainTrait(new Trait(trait.def, trait.degree));
                }
            }
        }
    }

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