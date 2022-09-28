using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace RareXenotypesSuccubus
{
    public class Hediff_LoveThrall : HediffWithComps
    {
        public Pawn master;
        public Faction previousFaction;
        public override bool ShouldRemove => false;
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            master.relations.AddDirectRelation(RX_DefOf.RX_Thrall, pawn);
            pawn.relations.AddDirectRelation(RX_DefOf.RX_Master, master);
            foreach (var lover in LovePartnerRelationUtility.ExistingLovePartners(pawn))
            {
                pawn.interactions.TryInteractWith(lover.otherPawn, RX_DefOf.Breakup);
            }
            master.relations.AddDirectRelation(RX_DefOf.RX_Thrall, pawn);
            pawn.relations.AddDirectRelation(RX_DefOf.RX_Master, master);
            master.relations.AddDirectRelation(PawnRelationDefOf.Lover, pawn);
            pawn.relations.AddDirectRelation(PawnRelationDefOf.Lover, master);
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
