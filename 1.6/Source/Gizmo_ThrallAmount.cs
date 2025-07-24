using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [StaticConstructorOnStartup]
    public class Gizmo_ThrallAmount : Gizmo
    {
        public Gene_PsychicEnthralling gene;

        public static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);
        public static readonly Color FilledBlockColor = GenColor.FromBytes(214, 125, 123);
        public static readonly Color ExcessBlockColor = Color.red;

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
			var baseRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			var rect = baseRect.ContractedBy(6f);

			Widgets.DrawWindowBackground(baseRect);
			var thrallCapacity = gene.LovethrallCapacity;
			var usedCapacity = gene.pawn.GetThrallAmount();
			var capacityText = $"{usedCapacity:F0} / {thrallCapacity:F0}";

			var taggedString = $"{"RS.LoveThralls".Translate().Colorize(ColoredText.TipSectionTitleColor)}: {capacityText}\n\n" + "RS.LoveThrallsDesc".Translate();
			TooltipHandler.TipRegion(baseRect, taggedString);

			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			var capacityRect = rect with { height = 20f }; 
			Widgets.Label(capacityRect, "RS.LoveThralls".Translate());
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(capacityRect, capacityText);
			Text.Anchor = TextAnchor.UpperLeft;

			var actualUsedCapacity = Mathf.Max(usedCapacity, thrallCapacity);
			var displayAreaRect = new Rect(rect.x, capacityRect.yMax + 6f, rect.width, rect.height - capacityRect.height - 6f);
			var columns = 2;
			var drawSize = Mathf.FloorToInt(displayAreaRect.height / columns);
			var rows = Mathf.FloorToInt(displayAreaRect.width / drawSize);
			var tries = 0;
			// Keep increasing the amount of columns until the amount of all positions is more than the actual used capacity
			while (columns * rows < actualUsedCapacity)
			{
				// Increase the amount of columns by 1
				columns++;
				// Precalculate the size of a single square
				drawSize = Mathf.FloorToInt(displayAreaRect.height / columns);
				// Recalculate the amount of squares based on the size of the square
				rows = Mathf.FloorToInt(displayAreaRect.width / drawSize);

				// Increment tries, give up after 1000
				tries++;
				if (tries >= 1000)
				{
					Log.Error($"Failed to fit lovethrall capacity cells into gizmo rect for {gene.pawn}.");
					return new GizmoResult(GizmoState.Clear);
				}
			}

			// One final row calculation to be extra sure, I suppose
			rows = Mathf.FloorToInt(displayAreaRect.width / drawSize);

			var offset = (displayAreaRect.width - rows * drawSize) / 2f;
			// Keep track of which square we're drawing, as each has a different color based if it's used/unused/over capacity
			var current = 0;

			for (var i = 0; i < columns; i++)
			{
				for (var j = 0; j < rows; j++)
				{
					current++;
					if (current <= actualUsedCapacity)
					{
						var drawRect = new Rect(displayAreaRect.x + j * drawSize + offset, displayAreaRect.y + i * drawSize, drawSize, drawSize).ContractedBy(2f);

						if (current <= usedCapacity)
						{
							Widgets.DrawRectFast(drawRect, current <= thrallCapacity ? FilledBlockColor : ExcessBlockColor);
						}
						else
						{
							Widgets.DrawRectFast(drawRect, EmptyBlockColor);
						}
					}
				}
			}

			return new GizmoResult(GizmoState.Clear);
        }

    }
}
