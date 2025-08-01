using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ReSpliceCharmweavers;

public class ThrallControlGroupGizmo : Gizmo
{
    public const int InRectPadding = 6;
    private const float Width = 130f;
    private const int IconButtonSize = 26;
    private const float BaseSelectedTexJump = 20f;
    private const float BaseSelectedTextScale = 0.8f;

    private ThrallControlGroup controlGroup;
    private List<ThrallControlGroup> mergedControlGroup;

    public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions => GetWorkModeOptions(controlGroup);

    public override bool Visible => controlGroup.ThrallsForReading.Count > 0 || Find.Selector.SelectedPawns.Count == 1;

    public override float Order => controlGroup.ThrallsForReading.Count > 0 ? -89 : 88;

    public override float GetWidth(float maxWidth) => Width;

    public ThrallControlGroupGizmo(ThrallControlGroup controlGroup)
    {
        this.controlGroup = controlGroup;
        Order = -89;
    }

    public override void GizmoUpdateOnMouseover()
    {
        base.GizmoUpdateOnMouseover();
        controlGroup.WorkMode.Worker.DrawControlGroupMouseOverExtra(controlGroup);
    }

    public override bool GroupsWith(Gizmo other)
    {
        if (other is not ThrallControlGroupGizmo otherGizmo)
            return false;
        if (controlGroup == otherGizmo.controlGroup)
            return true;
        if (controlGroup.Gene == otherGizmo.controlGroup.Gene && controlGroup.ThrallsForReading.Count == 0 && otherGizmo.controlGroup.ThrallsForReading.Count == 0)
            return true;

        if (mergedControlGroup.NotNullAndContains(otherGizmo.controlGroup))
            mergedControlGroup.Remove(otherGizmo.controlGroup);
        return false;
    }

    public override void MergeWith(Gizmo other)
    {
        if (other is not ThrallControlGroupGizmo otherGizmo)
        {
            Log.ErrorOnce($"Tried to merge {nameof(ThrallControlGroupGizmo)} with unexpected type: {other?.GetType()}", 105251612);
            return;
        }

        if (controlGroup != otherGizmo.controlGroup)
        {
            mergedControlGroup ??= [];
            if (!mergedControlGroup.Contains(otherGizmo.controlGroup))
                mergedControlGroup.Add(otherGizmo.controlGroup);
        }
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var mainRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        var paddingRect = mainRect.ContractedBy(InRectPadding);

        var isMouseOver = Mouse.IsOver(paddingRect);
        var thrallsForReading = controlGroup.ThrallsForReading;

        using (new TextBlock(parms.lowLight ? Command.LowLightBgColor : Color.white))
            GenUI.DrawTextureWithMaterial(mainRect, parms.shrunk ? Command.BGTexShrunk : Command.BGTex, parms.lowLight || thrallsForReading.Count <= 0 ? TexUI.GrayscaleGUI : null);

        using var _ = new TextBlock(GameFont.Small, TextAnchor.UpperLeft);

        var groupText = !mergedControlGroup.NullOrEmpty() ? "RS.Groups".Translate() : "RS.Group".Translate();
        groupText += $" {controlGroup.Index}";
        if (!mergedControlGroup.NullOrEmpty())
        {
            mergedControlGroup.SortBy(x => x.Index);
            for (var i = 0; i < mergedControlGroup.Count; i++)
                groupText  += $", {mergedControlGroup[i].Index}";
        }
        groupText = groupText.Truncate(paddingRect.width);

        var textSize = Text.CalcSize(groupText);
        var groupTextRect = paddingRect with
        {
            width = textSize.x,
            height = textSize.y,
        };
        Widgets.Label(groupTextRect, groupText);

        if (thrallsForReading.Count <= 0)
        {
            using var __ = new TextBlock(TextAnchor.MiddleCenter, ColoredText.SubtleGrayColor);
            Widgets.Label(paddingRect, $"({"RS.NoThralls".Translate()})");
            return new GizmoResult(GizmoState.Clear);
        }

        if (Mouse.IsOver(groupTextRect))
        {
            Widgets.DrawHighlight(groupTextRect);
            if (Widgets.ButtonInvisible(groupTextRect))
            {
                Find.Selector.ClearSelection();
                for (var i = 0; i < thrallsForReading.Count; i++)
                    Find.Selector.Select(thrallsForReading[i]);
            }
        }

        var isMouseOverWorkMode = false;
        var workModeButtonRect = new Rect(mainRect.x + mainRect.width - (IconButtonSize + InRectPadding), mainRect.y + InRectPadding, IconButtonSize, IconButtonSize);
        Widgets.DrawTextureFitted(workModeButtonRect, controlGroup.WorkMode.uiIcon, 1f);
        if (Mouse.IsOver(workModeButtonRect))
        {
            isMouseOverWorkMode = true;
            Widgets.DrawHighlight(workModeButtonRect);
        }

        var pawnsRect = new Rect(paddingRect.x, paddingRect.y + IconButtonSize + 4f, paddingRect.width, paddingRect.height - (IconButtonSize + 4f));
        var pawnIconSize = pawnsRect.height;
        var columns = 0;
        var rows = 0;
        for (; pawnIconSize >= 0f; pawnIconSize -= 1)
        {
            columns = Mathf.FloorToInt(pawnsRect.width / pawnIconSize);
            rows = Mathf.FloorToInt(pawnsRect.height / pawnIconSize);
            if (columns * rows >= thrallsForReading.Count)
                break;
        }

        var horizontalOffset = (pawnsRect.width - columns * pawnIconSize) / 2f;
        var verticalOffset = (pawnsRect.height - rows * pawnIconSize) / 2f;
        var totalPawns = 0;
        for (var i = 0; i < columns; i++)
        {
            var pawnsThisRow = 0;

            while (pawnsThisRow < columns && totalPawns < thrallsForReading.Count)
            {
                var thrallIconRect = new Rect(pawnsRect.x + pawnsThisRow * pawnIconSize + horizontalOffset, pawnsRect.y + i * pawnIconSize + verticalOffset, pawnIconSize, pawnIconSize);
                var thrall = thrallsForReading[totalPawns];
                var texture = PortraitsCache.Get(thrall, thrallIconRect.size, Rot4.South);

                GUI.DrawTexture(thrallIconRect, texture);
                if (Mouse.IsOver(thrallIconRect))
                {
                    Widgets.DrawHighlight(thrallIconRect);
                    MouseoverSounds.DoRegion(thrallIconRect, SoundDefOf.Mouseover_Command);
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (Event.current.shift)
                            Find.Selector.Select(thrall);
                        else
                            CameraJumper.TryJumpAndSelect(thrall);
                    }
                    TargetHighlighter.Highlight(thrall, colonistBar: false);
                }

                if (Find.Selector.IsSelected(thrallsForReading[totalPawns]))
                    SelectionDrawerUtility.DrawSelectionOverlayOnGUI(thrallsForReading[totalPawns], thrallIconRect, BaseSelectedTextScale / columns, BaseSelectedTexJump);

                totalPawns++;
                pawnsThisRow++;
            }

            if (totalPawns >= thrallsForReading.Count)
                break;
        }

        if (Find.WindowStack.FloatMenu == null)
        {
            TooltipHandler.TipRegion(mainRect, () =>
            {
                var text = ("RS.ControlGroup".Translate() + " #" + controlGroup.Index.ToString()).Colorize(ColoredText.TipSectionTitleColor) + "\n\n";
                text += $"{("RS.CurrentThrallWorkMode".Translate() + ": " + controlGroup.WorkMode.LabelCap).Colorize(ColoredText.TipSectionTitleColor)}\n{controlGroup.WorkMode.description}\n\n";
                text += "RS.AssignedThralls".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n" + controlGroup.ThrallsForReading.Select(x => x.LabelCap).ToLineList(" - ");

                return text;
            }, 2018572371);
        }

        if (isMouseOverWorkMode && Event.current.type == EventType.MouseDown)
            return new GizmoResult(GizmoState.OpenedFloatMenu, Event.current);
        if (isMouseOver)
            return new GizmoResult(GizmoState.Mouseover);
        return new GizmoResult(GizmoState.Clear);
    }

    public IEnumerable<FloatMenuOption> GetWorkModeOptions(ThrallControlGroup controlGroup)
    {
        foreach (var wm in DefDatabase<ThrallWorkModeDef>.AllDefsListForReading.OrderBy(x => x.uiOrder))
        {
            yield return new FloatMenuOption(wm.LabelCap, () => controlGroup.SetWorkMode(wm), wm.uiIcon, Color.white)
            {
                tooltip = new TipSignal(wm.description, wm.index ^ -167302435)
            };
        }
    }
}