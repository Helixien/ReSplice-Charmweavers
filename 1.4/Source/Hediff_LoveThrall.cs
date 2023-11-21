﻿using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;
using VFECore;

namespace ReSpliceCharmweavers
{
    public class Hediff_LoveThrall : HediffWithComps
    {
        public Pawn master;
        public Faction previousFaction;
        public bool gainedGayTrait;
        public override bool ShouldRemove => master.DestroyedOrNull() || master.Dead;
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            master.relations.AddDirectRelation(RS_DefOf.RX_Thrall, pawn);
            pawn.relations.AddDirectRelation(RS_DefOf.RX_Master, master);
            foreach (var loveRel in LovePartnerRelationUtility.ExistingLovePartners(pawn))
            {
                if (loveRel.otherPawn != master)
                {
                    pawn.interactions.TryInteractWith(loveRel.otherPawn, RS_DefOf.Breakup);
                }
            }
            if (master.relations.DirectRelationExists(PawnRelationDefOf.Lover, pawn) is false)
            {
                master.relations.AddDirectRelation(PawnRelationDefOf.Lover, pawn);
            }
            if (pawn.relations.DirectRelationExists(PawnRelationDefOf.Lover, master) is false)
            {
                pawn.relations.AddDirectRelation(PawnRelationDefOf.Lover, master);
            }

            FleckMaker.Static(master.Position, master.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
            FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.5f);

            Find.LetterStack.ReceiveLetter("RX.NewThrall".Translate(pawn.Named("PAWN")), "RX.NewThrallDesc".Translate(master.Named("CASTER"), pawn.Named("TARGET")),
                LetterDefOf.NeutralEvent, pawn);

            if (master.genes.HasGene(RS_DefOf.RS_LoveFeed))
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

        public override void PostRemoved()
        {
            base.PostRemoved();
            if (previousFaction != null && pawn.Faction != previousFaction)
            {
                pawn.SetFaction(previousFaction);
            }

            var relation = pawn.relations.GetDirectRelation(RS_DefOf.RX_Master, master);
            if (relation != null)
            {
                pawn.relations.RemoveDirectRelation(relation);
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref master, "master");
            Scribe_References.Look(ref previousFaction, "previousFaction");
            Scribe_Values.Look(ref gainedGayTrait, "gainedGayTrait");

        }
    }
}
