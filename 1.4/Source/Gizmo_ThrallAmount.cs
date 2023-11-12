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

        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

        private static readonly Texture2D TargetLevelArrow = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated");

        private const float ArrowScale = 0.5f;

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
            Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(1523289473, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect;
                Rect rect2 = (rect = overRect.AtZero().ContractedBy(6f));
                rect.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect, "RS.ThrallAmount".Translate());
                Rect rect3 = rect2;
                rect3.yMin = overRect.height / 2f;
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                var thrallAmount = pawn.relations.DirectRelations.Where(x => x.def == RS_DefOf.RX_Thrall).Count();
                Widgets.Label(rect3, thrallAmount.ToString() + " / " + ReSpliceCharmweaversSettings.maxThrallAmount);
                Text.Anchor = TextAnchor.UpperLeft;
            });
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
