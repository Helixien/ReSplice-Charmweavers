using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers
{
    public class Hediff_LoveThrall : HediffWithComps
    {
        public Pawn master;
        public Faction previousFaction;
        public override bool ShouldRemove => master.DestroyedOrNull() || master.Dead;

        public override void CopyFrom(Hediff other)
        {
            base.CopyFrom(other);
            var otherHediff = other as Hediff_LoveThrall;
            master = otherHediff.master;
            previousFaction = otherHediff.previousFaction;
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

            if (pawn.Map != null)
            {
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
                    Find.LetterStack.ReceiveLetter("RS.NewThrall".Translate(pawn.Named("PAWN")), "RS.NewThrallDesc".Translate(master.Named("CASTER"), pawn.Named("TARGET")),
                       LetterDefOf.NeutralEvent, pawn);
                }

                MoteMaker.MakeAttachedOverlay(pawn, RS_DefOf.RS_EnthralledMote, Vector2.zero);
            }

            if (pawn.genes != null)
            {
                if (master.genes.HasActiveGene(RS_DefOf.RS_LoveFeed))
                {
                    _ = pawn.relations.GetPregnancyApproachForPartner(master);
                    pawn.relations.GetAdditionalPregnancyApproachData().partners[master] = RS_DefOf.RS_LovinForHemogen;
                    master.relations.GetAdditionalPregnancyApproachData().partners[pawn] = RS_DefOf.RS_LovinForHemogen;
                }

                var bisexual = false;
                var gay = false;
                if (pawn.story?.traits != null)
                {
                    if (pawn.story.traits.HasTrait(TraitDefOf.Bisexual))
                        bisexual = true;
                    else if (pawn.story.traits.HasTrait(TraitDefOf.Gay))
                        gay = true;
                }

                // Honestly, if gender of either pawn is none, just add bisexual and be done with it.
                // If the pawn is bisexual already, also just be done with it.
                if (bisexual || pawn.gender == Gender.None || master.gender == Gender.None)
                    pawn.genes.AddGene(RS_DefOf.RS_LovethrallBisexual, true);
                // Straight relation
                else if (pawn.gender != master.gender)
                {
                    // If pawn is gay, 20% chance to force bisexual. Straight otherwise.
                    if (gay && Rand.Chance(0.2f))
                        pawn.genes.AddGene(RS_DefOf.RS_LovethrallBisexual, true);
                    else
                        pawn.genes.AddGene(RS_DefOf.RS_LovethrallStraight, true);
                }
                // Gay relation
                else
                {
                    // If pawn is not gay, 20% chance for bisexual. Gay otherwise.
                    if (!gay && Rand.Chance(0.2f))
                        pawn.genes.AddGene(RS_DefOf.RS_LovethrallBisexual, true);
                    else
                        pawn.genes.AddGene(RS_DefOf.RS_LovethrallGay, true);
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

                if (pawn.genes != null)
                {
                    ClearGenes(pawn.genes.Xenogenes);
                    ClearGenes(pawn.genes.Endogenes);
                }
            }
            finally
            {
                recursionTrap = false;
            }

            void ClearGenes(List<Gene> genes)
            {
                for (var i = genes.Count - 1; i >= 0; i--)
                {
                    var gene = genes[i];
                    if (gene is Gene_Thrall)
                        pawn.genes.RemoveGene(gene);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref master, "master");
            Scribe_References.Look(ref previousFaction, "previousFaction");

            if (Scribe.mode == LoadSaveMode.PostLoadInit && master != null)
            {
                var gene = master.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>();
                if (gene != null && gene.GetControlGroup(pawn) == null)
                {
                    gene.controlGroups[0].Assign(pawn);
                }
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
                yield return gizmo;

            if (master != null)
            {
                yield return new Command_Action
                {
                    defaultLabel = "RS.CommandSelectMaster".Translate(),
                    defaultDesc = "RS.CommandSelectMasterDesc".Translate(master.Named("PAWN")),
                    icon = Core.SelectMasterIcon,
                    action = () =>
                    {
                        Find.Selector.ClearSelection();
                        Find.Selector.Select(master);
                    },
                    onHover = () => GenDraw.DrawArrowPointingAt(master.TrueCenter()),
                };

                if (pawn.Faction == Faction.OfPlayer)
                {
                    var controlGroup = master.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>()?.GetControlGroup(pawn);
                    if (controlGroup != null)
                        yield return new ThrallControlGroupGizmo(controlGroup);

                    if (AssignThrallToGroupUtility.CanAssignPawn(pawn))
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = "RS.AssignToGroup".Translate(),
                            defaultDesc = "RS.AssignToGroupDesc".Translate(),
                            icon = ContentFinder<Texture2D>.Get("UI/Icons/Gizmos/AssignToGroupThrall"),
                            action = AssignThrallToGroupUtility.CreateAssignThrallsFloatMenu,
                        };
                    }
                }
            }
        }
    }
}
