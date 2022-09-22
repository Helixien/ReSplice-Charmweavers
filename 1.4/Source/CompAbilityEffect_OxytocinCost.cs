using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RareXenotypesSuccubus
{
    public class CompProperties_AbilityOxytocinCost : CompProperties_AbilityEffect
    {
        public float oxytocinCost;
        public CompProperties_AbilityOxytocinCost()
        {
            compClass = typeof(CompAbilityEffect_OxytocinCost);
        }

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return (string)("RX.AbilityOxytocinCost".Translate() + ": ") + Mathf.RoundToInt(oxytocinCost * 100f);
        }
    }

    public class CompAbilityEffect_OxytocinCost : CompAbilityEffect
    {
        public new CompProperties_AbilityOxytocinCost Props => (CompProperties_AbilityOxytocinCost)props;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            OxytocinUtility.OffsetOxytocin(parent.pawn, 0f - Props.oxytocinCost);
        }

        public override bool GizmoDisabled(out string reason)
        {
            Gene_Oxytocin gene_Oxytocin = parent.pawn.genes?.GetFirstGeneOfType<Gene_Oxytocin>();
            if (gene_Oxytocin == null)
            {
                reason = "RX.AbilityDisabledNoOxytocinGene".Translate(parent.pawn);
                return true;
            }
            if (gene_Oxytocin.Value < Props.oxytocinCost)
            {
                reason = "RX.AbilityDisabledNoOxytocin".Translate(parent.pawn);
                return true;
            }
            reason = null;
            return false;
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if ((parent.pawn.genes?.GetFirstGeneOfType<Gene_Oxytocin>()).Value < Props.oxytocinCost)
            {
                return false;
            }
            return true;
        }
    }
}
