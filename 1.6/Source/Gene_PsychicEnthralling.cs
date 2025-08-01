using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    public class Gene_PsychicEnthralling : Gene
    {
        public List<ThrallControlGroup> controlGroups = [];
        public List<ThrallControlGroupGizmo> controlGroupGizmos = [];

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
            Scribe_Collections.Look(ref controlGroups, nameof(controlGroups), LookMode.Deep, this);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                controlGroups ??= [];
                Notify_ControlGroupAmountMayChanged();
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            gizmo ??= new Gizmo_ThrallAmount
            {
                gene = this
            };
            yield return gizmo;

            foreach (var controlGroupGizmo in controlGroupGizmos)
                yield return controlGroupGizmo;

            yield return new Command_Action
            {
                defaultLabel = "RS.CommandSelectAllThralls".Translate(),
                defaultDesc = "RS.CommandSelectAllThrallsDesc".Translate(pawn.Named("PAWN")),
                icon = Core.SelectAllThrallsIcon,
                Order = -87f,
                action = () =>
                {
                    Find.Selector.ClearSelection();
                    foreach (var relation in pawn.relations.DirectRelations.Where(x => x.def == RS_DefOf.RS_Thrall))
                    {
                        Find.Selector.Select(relation.otherPawn);
                    }
                },
            };
        }

        private void Notify_ControlGroupAmountMayChanged()
        {
            const int TempMaxControlGroups = 2;

            var totalGroups = TempMaxControlGroups;

            while (controlGroups.Count < totalGroups)
                controlGroups.Add(new ThrallControlGroup(this));

            List<Pawn> thralls = null;
            while (controlGroups.Count > totalGroups)
            {
                var lastGroup = controlGroups[^1];
                if (controlGroups.Count > 1)
                {
                    if (!lastGroup.ThrallsForReading.NullOrEmpty())
                    {
                        thralls ??= [];
                        thralls.AddRange(lastGroup.ThrallsForReading);
                    }
                }
                else
                {
                    Log.Warning("Removed last thrall control group");
                }

                controlGroups.RemoveAt(controlGroups.Count - 1);
            }

            if (thralls != null && controlGroups.Count > 0)
            {
                foreach (var thrall in thralls)
                    controlGroups[0].Assign(thrall);
            }

            controlGroupGizmos.Clear();
            foreach (var group in controlGroups)
                controlGroupGizmos.Add(new ThrallControlGroupGizmo(group));
        }

        public ThrallControlGroup GetControlGroup(Pawn thrall)
        {
            for (var i = 0; i < controlGroups.Count; i++)
            {
                var group = controlGroups[i];
                if (group.ThrallsForReading.Contains(thrall))
                    return group;
            }

            return null;
        }
    }
}
