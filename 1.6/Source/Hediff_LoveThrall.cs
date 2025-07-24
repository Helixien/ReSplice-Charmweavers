using RimWorld;
using System.Collections.Generic;
using System.Text;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers
{
    public class Hediff_LoveThrall : HediffWithComps
    {
        public Pawn master;
        public Faction previousFaction;
        public bool gainedGayTrait;
        public override bool ShouldRemove => master.DestroyedOrNull() || master.Dead;

        public override void CopyFrom(Hediff other)
        {
            base.CopyFrom(other);
            var otherHediff = other as Hediff_LoveThrall;
            master = otherHediff.master;
            previousFaction = otherHediff.previousFaction;
            gainedGayTrait = otherHediff.gainedGayTrait;
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            master.relations.AddDirectRelation(RS_DefOf.RS_Thrall, pawn);
            pawn.relations.AddDirectRelation(RS_DefOf.RS_Master, master);
            foreach (var loveRel in LovePartnerRelationUtility.ExistingLovePartners(pawn))
            {
                if (loveRel.otherPawn != master)
                {
                    pawn.interactions.TryInteractWith(loveRel.otherPawn, RS_DefOf.Breakup);
                }
            }

            if (LovePartnerRelationUtility.LovePartnerRelationExists(master, pawn) is false)
            {
                master.relations.AddDirectRelation(PawnRelationDefOf.Lover, pawn);
            }

            if (master.Map != null)
            {
                FleckMaker.Static(master.Position, master.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
            }

            if (pawn.Map != null && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
                Find.LetterStack.ReceiveLetter("RS.NewThrall".Translate(pawn.Named("PAWN")), "RS.NewThrallDesc".Translate(master.Named("CASTER"), pawn.Named("TARGET")),
                   LetterDefOf.NeutralEvent, pawn);
            }

            if (master.genes.HasActiveGene(RS_DefOf.RS_LoveFeed))
            {
                _ = pawn.relations.GetPregnancyApproachForPartner(master);
                pawn.relations.GetAdditionalPregnancyApproachData().partners[master] = RS_DefOf.RS_LovinForHemogen;
                master.relations.GetAdditionalPregnancyApproachData().partners[pawn] = RS_DefOf.RS_LovinForHemogen;
            }

            if (master.gender == pawn.gender)
            {
                if (pawn.story.traits.HasTrait(TraitDefOf.Gay) is false)
                {
                    pawn.story.traits.GainTrait(new Trait(TraitDefOf.Gay, 0, forced: true));
                    gainedGayTrait = true;
                }
            }
        }


        public bool recursionTrap;
        public override void PostRemoved()
        {
            base.PostRemoved();
            if (recursionTrap)
            {
                return;
            }

            try
            {
                recursionTrap = true;
                if (previousFaction != null && pawn.Faction != previousFaction)
                {
                    pawn.SetFaction(previousFaction);
                }
                var masterRelation = pawn.relations.GetDirectRelation(RS_DefOf.RS_Master, master);
                if (masterRelation != null)
                {
                    pawn.relations.RemoveDirectRelation(masterRelation);
                }
                var loverRelation = pawn.relations.GetDirectRelation(PawnRelationDefOf.Lover, master);
                if (loverRelation != null)
                {
                    pawn.relations.RemoveDirectRelation(loverRelation);
                }

                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(RS_DefOf.RS_BrokenEnthrallment);
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(RS_DefOf.RS_EnthralledMe, master);
                if (gainedGayTrait)
                {
                    var gayTrait = pawn.story.traits.GetTrait(TraitDefOf.Gay);
                    if (gayTrait != null)
                    {
                        pawn.story.traits.RemoveTrait(gayTrait);
                    }
                }
            }
            finally
            {
                recursionTrap = false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref master, "master");
            Scribe_References.Look(ref previousFaction, "previousFaction");
            Scribe_Values.Look(ref gainedGayTrait, "gainedGayTrait");

        }
    }
}
