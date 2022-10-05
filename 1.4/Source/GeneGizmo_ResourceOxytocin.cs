using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RareXenotypesSuccubus
{
    [StaticConstructorOnStartup]
    public class GeneGizmo_ResourceOxytocin : GeneGizmo_Resource
    {
        private static readonly Texture2D OxytocinCostTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.66f));

        private const float TotalPulsateTime = 0.85f;

        private List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new();
        public GeneGizmo_ResourceOxytocin(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barHighlightColor)
            : base(gene, drainGenes, barColor, barHighlightColor)
        {
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            var result = base.GizmoOnGUI(topLeft, maxWidth, parms);
            float num = Mathf.Repeat(Time.time, 0.85f);
            float num2 = 1f;
            if (num < 0.1f)
            {
                num2 = num / 0.1f;
            }
            else if (num >= 0.25f)
            {
                num2 = 1f - ((num - 0.25f) / 0.6f);
            }
            if (((MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow)?.LastMouseoverGizmo is Command_Ability command_Ability && gene.Max != 0f)
            {
                foreach (var effectComp in command_Ability.Ability.EffectComps)
                {
                    if (effectComp is CompAbilityEffect_OxytocinCost compAbilityEffect_OxytocinCost && compAbilityEffect_OxytocinCost.Props.oxytocinCost > float.Epsilon)
                    {
                        var rect = barRect.ContractedBy(3f);
                        float width = rect.width;
                        float num3 = gene.Value / gene.Max;
                        rect.xMax = rect.xMin + (width * num3);
                        float num4 = Mathf.Min(compAbilityEffect_OxytocinCost.Props.oxytocinCost / gene.Max, 1f);
                        rect.xMin = Mathf.Max(rect.xMin, rect.xMax - (width * num4));
                        GUI.color = new Color(1f, 1f, 1f, num2 * 0.7f);
                        GenUI.DrawTextureWithMaterial(rect, OxytocinCostTex, null);
                        GUI.color = Color.white;
                        return result;
                    }
                }
                return result;
            }
            return result;
        }
        public override string GetTooltip()
        {
            tmpDrainGenes.Clear();
            string text = $"{gene.ResourceLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}: {gene.ValueForDisplay} / {gene.MaxForDisplay}";
            if (!drainGenes.NullOrEmpty())
            {
                float num = 0f;
                foreach (var drainGene in drainGenes)
                {
                    if (drainGene.CanOffset)
                    {
                        tmpDrainGenes.Add(new Pair<IGeneResourceDrain, float>(drainGene, drainGene.ResourceLossPerDay));
                        num += drainGene.ResourceLossPerDay;
                    }
                }
                if (num != 0f)
                {
                    string text2 = (num < 0f) ? "RegenerationRate".Translate() : "DrainRate".Translate();
                    text = text + "\n\n" + text2 + ": " + "PerDay".Translate(Mathf.Abs(Mathf.RoundToInt(num * 100f))).Resolve();
                    foreach (var tmpDrainGene in tmpDrainGenes)
                    {
                        text = text + "\n  - " + tmpDrainGene.First.DisplayLabel.CapitalizeFirst() + ": " + "PerDay".Translate(Mathf.RoundToInt((0f - tmpDrainGene.Second) * 100f).ToStringWithSign()).Resolve();
                    }
                }
            }
            if (!gene.def.resourceDescription.NullOrEmpty())
            {
                text = text + "\n\n" + gene.def.resourceDescription.Formatted(gene.pawn.Named("PAWN")).Resolve();
            }
            return text;
        }
    }
}
