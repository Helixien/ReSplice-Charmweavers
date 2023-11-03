using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ReSpliceCharmweavers
{
    public class Hediff_LoveThrall : HediffWithComps
    {
        public Pawn master;
        public Faction previousFaction;
        public override bool ShouldRemove => false;
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
        }

        public void MakeBerserk()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk,
                "RX.BerserkThrallStateReason".Translate(master));
            if (previousFaction != null && pawn.Faction != previousFaction)
            {
                pawn.SetFaction(previousFaction);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref master, "master");
            Scribe_References.Look(ref previousFaction, "previousFaction");

        }
    }
}
