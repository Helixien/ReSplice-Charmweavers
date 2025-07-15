using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ReSpliceCharmweavers
{
    public class Gene_PsychicEnthralling : Gene
    {
        public Gizmo_ThrallAmount gizmo;

        private int loveThrallCapacityOffset;

        public int LovethrallCapacity => ReSpliceCharmweaversSettings.maxThrallAmount + loveThrallCapacityOffset;

        public void OffsetCapacity(int offset, bool sendNotification = true)
        {
            int num = LovethrallCapacity;
            loveThrallCapacityOffset += offset;
            if (sendNotification && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("RS.MessageLovethrallCapacityChanged".Translate(pawn.Named("PAWN"), num.Named("OLD"), LovethrallCapacity.Named("NEW")), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref loveThrallCapacityOffset, "loveThrallCapacityOffset");
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            gizmo ??= new Gizmo_ThrallAmount
            {
                gene = this
            };
            yield return gizmo;
        }
    }
}
