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
        private int loveThrallControlGroupCapacityOffset;

        public int LovethrallCapacity => ReSpliceCharmweaversSettings.maxThrallAmount + loveThrallCapacityOffset;
        public int LovethrallControlGroupCapacity => ReSpliceCharmweaversSettings.maxThrallControlGroupAmount + loveThrallControlGroupCapacityOffset;

        public void OffsetCapacity(int offset, bool sendNotification = true)
        {
            int num = LovethrallCapacity;
            loveThrallCapacityOffset += offset;
            Notify_ControlGroupAmountMayChanged();
            if (sendNotification && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("RS.MessageLovethrallCapacityChanged".Translate(pawn.Named("PAWN"), num.Named("OLD"), LovethrallCapacity.Named("NEW")), pawn, MessageTypeDefOf.PositiveEvent);
            }
        }

        public void OffsetControlGroupCapacity(int offset, bool sendNotification = true)
        {
            // Unused in the mod itself.
            // Currently, this exists for mod compatibility in case some mods want to increase the offset.
            int num = LovethrallControlGroupCapacity;
            loveThrallControlGroupCapacityOffset += offset;
            if (sendNotification && PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("RS.MessageLovethrallControlGroupCapacityChanged".Translate(pawn.Named("PAWN"), num.Named("OLD"), LovethrallCapacity.Named("NEW")), pawn, MessageTypeDefOf.PositiveEvent);
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

        public void Notify_ControlGroupAmountMayChanged()
        {
            var totalGroups = LovethrallControlGroupCapacity;
            if (totalGroups < 1)
            {
                totalGroups = 1;
                Log.ErrorOnce(
                    $"{pawn.Name} has too little thrall control groups. They have {totalGroups}, while they need at least 1. " +
                    $"Base is {ReSpliceCharmweaversSettings.maxThrallControlGroupAmount} while the offset is {loveThrallControlGroupCapacityOffset}",
                    Gen.HashCombineInt(pawn.thingIDNumber, -2091729361));
            }

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

        public override void PostAdd()
        {
            base.PostAdd();
            Reset();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            Reset();
            Pawn_Kill_Patch.RemoveThrallRelationships(pawn);
        }

        public override void Reset()
        {
            base.Reset();

            loveThrallCapacityOffset = 0;
            loveThrallControlGroupCapacityOffset = 0;
            Notify_ControlGroupAmountMayChanged();
        }
    }
}