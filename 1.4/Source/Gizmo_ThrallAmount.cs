using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [StaticConstructorOnStartup]
    public class Gizmo_ThrallAmount : Gizmo
    {
        public Pawn pawn;

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
            var thrallAmount = pawn.GetThrallAmount();
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(8f);
            Widgets.DrawWindowBackground(rect);
            Text.Font = GameFont.Small;
            Rect labelRect = rect2;
            labelRect.height = Text.LineHeight;
            Widgets.Label(labelRect, "RS.ThrallAmount".Translate());
            rect2.yMin += labelRect.height + 8f;
            var barRect = rect2;
            Widgets.FillableBar(barRect, thrallAmount / (float)ReSpliceCharmweaversSettings.maxThrallAmount, FullBarTex, EmptyBarTex, doBorder: true);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, thrallAmount.ToString() + " / " + ReSpliceCharmweaversSettings.maxThrallAmount);
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
