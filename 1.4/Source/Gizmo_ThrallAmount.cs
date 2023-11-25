using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [StaticConstructorOnStartup]
    public class Gizmo_ThrallAmount : Gizmo
    {
        public Gene_PsychicEnthralling gene;

        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(Core.SuccubColor);

        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

        public Gizmo_ThrallAmount()
        {
            Order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            var thrallAmount = gene.pawn.GetThrallAmount();
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(8f);
            Widgets.DrawWindowBackground(rect);
            Text.Font = GameFont.Small;
            Rect labelRect = rect2;
            labelRect.height = Text.LineHeight;
            Widgets.Label(labelRect, "RS.ThrallAmount".Translate());
            rect2.yMin += labelRect.height + 8f;
            var barRect = rect2;
            Widgets.FillableBar(barRect, thrallAmount / (float)gene.LovethrallCapacity, FullBarTex, EmptyBarTex, doBorder: true);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, thrallAmount.ToString() + " / " + gene.LovethrallCapacity);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                string tip = "RS.ThrallAmountTooltip".Translate();
                if (!tip.NullOrEmpty())
                {
                    TooltipHandler.TipRegion(rect, tip);
                }
            }
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
