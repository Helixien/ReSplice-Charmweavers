using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RareXenotypesSuccubus
{
    public class Gene_Oxytocin : Gene_Resource, IGeneResourceDrain
    {
        public Gene_Resource Resource => this;
        public Pawn Pawn => pawn;
        public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";
        public float ResourceLossPerDay => def.resourceLossPerDay;
        public override float InitialResourceMax => 1f;
        public override float MinLevelForAlert => 0.15f;
        public override Color BarColor => Core.SuccubColor;
        public bool CanOffset => true;
        public override void PostAdd()
        {
            base.PostAdd();
            Reset();
        }

        public override void Tick()
        {
            base.Tick();
            OxytocinUtility.TickResourceDrain(this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            foreach (var resourceDrainGizmo in OxytocinUtility.GetResourceDrainGizmos(this))
            {
                yield return resourceDrainGizmo;
            }
        }

        public override float Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                if (Value <= 0)
                {
                    pawn.Kill(null);
                }
            }
        }

        public override Color BarHighlightColor => new ColorInt(145, 42, 42).ToColor;
    }
}
