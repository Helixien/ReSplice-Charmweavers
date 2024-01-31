using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ReSpliceCharmweavers
{
    public class CompProperties_UseEffectOffsetLovethrallCapacity : CompProperties_UseEffect
    {
        public int offset;

        public CompProperties_UseEffectOffsetLovethrallCapacity()
        {
            compClass = typeof(CompUseEffect_OffsetLovethrallCapacity);
        }
    }

    public class CompUseEffect_OffsetLovethrallCapacity : CompUseEffect
    {
        private CompProperties_UseEffectOffsetLovethrallCapacity Props => (CompProperties_UseEffectOffsetLovethrallCapacity)props;

        public override void DoEffect(Pawn user)
        {
            if (ModsConfig.BiotechActive)
            {
                base.DoEffect(user);
                user.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>()?.OffsetCapacity(Props.offset);
            }
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (p.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>() == null)
            {
                return "RS.CannotLovethrall".Translate();
            }
            return base.CanBeUsedBy(p);
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (ModsConfig.BiotechActive)
            {
                yield return new StatDrawEntry(StatCategoryDefOf.BasicsNonPawnImportant, "RS.LovethrallCapacity".Translate().CapitalizeFirst(), Props.offset.ToStringWithSign(), "RS.LovethrallCapacityDesc".Translate(), 1010);
            }
        }
    }
}
