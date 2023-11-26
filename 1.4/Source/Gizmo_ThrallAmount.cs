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

        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(Core.ThrallColor);

        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

        public Gizmo_ThrallAmount()
        {
            Order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 150f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect position = rect.ContractedBy(6f);
            float num = position.height / 3f;
            Widgets.DrawWindowBackground(rect);
            GUI.BeginGroup(position);
            Widgets.Label(new Rect(0f, 0f, position.width, num), "RS.PsychicEnthrallment".Translate().CapitalizeFirst());
            DrawOnGUI(new Rect(0f, num, position.width, num + 2f), 2f, new Rect(0f, 0f, position.width, num * 2f));
            Rect rect2 = new Rect(0f, num * 2f, position.width, Text.LineHeight);
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect2, string.Format("{0}: {1} / {2}", "RS.LoveThralls".Translate().CapitalizeFirst(), gene.pawn.GetThrallAmount(), gene.LovethrallCapacity));
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
            return new GizmoResult(GizmoState.Clear);
        }

        public void DrawOnGUI(Rect rect, float customMargin = -1f, Rect? rectForTooltip = null)
        {
            int thrallAmount = gene.pawn.GetThrallAmount();

            if (rect.height > 70f)
            {
                float num = (rect.height - 70f) / 2f;
                rect.height = 70f;
                rect.y += num;
            }
            Rect rect2 = rectForTooltip ?? rect;
            if (Mouse.IsOver(rect2))
            {
                Widgets.DrawHighlight(rect2);
            }
            if (Mouse.IsOver(rect2))
            {
                var tooltip = "RS.PsychicEnthrallmentDesc".Translate(thrallAmount, gene.LovethrallCapacity, gene.pawn.Named("PAWN"));
                TooltipHandler.TipRegion(rect2, new TipSignal(() => tooltip, rect2.GetHashCode()));
            }
            float num2 = 14f;
            float num3 = ((customMargin >= 0f) ? customMargin : (num2 + 15f));
            if (rect.height < 50f)
            {
                num2 *= Mathf.InverseLerp(0f, 50f, rect.height);
            }
            Rect rect3 = rect;
            rect3 = new Rect(rect3.x + num3, rect3.y, rect3.width - num3 * 2f, rect3.height - num2);
            Rect rect6 = rect3;
            float num4 = 1f;
            rect6.width *= num4;
            Rect barRect = Widgets.FillableBar(rect6, thrallAmount / (float)gene.LovethrallCapacity);
            Text.Font = GameFont.Small;
        }

    }
}
