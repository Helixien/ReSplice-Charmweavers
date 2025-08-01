using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers;

public class ThrallWorkModeDrawer
{
    private const float MouseoverLineWidth = 0.1f;
    private const float CircleOutlineRadius = 0.5f;
    private static readonly Vector3 IconScale = Vector3.one * 0.5f;

    public ThrallWorkModeDef def;
    private Material iconMat;

    protected virtual bool DrawIconAtTarget => true;

    public virtual void DrawControlGroupMouseOverExtra(ThrallControlGroup group)
    {
        var target = GetTargetForLine(group);
        var map = Find.CurrentMap;

        if (target.IsValid && target.Map == map)
        {
            var thralls = group.ThrallsForReading;
            var posShifted = target.Cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MoteOverhead);

            for (var i = 0; i < thralls.Count; i++)
            {
                var thrall = thralls[i];
                if (thrall.Map == map)
                {
                    GenDraw.DrawLineBetween(posShifted, thrall.DrawPos, SimpleColor.White, MouseoverLineWidth);
                    GenDraw.DrawCircleOutline(thrall.DrawPos, CircleOutlineRadius);
                }
            }

            if (DrawIconAtTarget)
            {
                iconMat ??= MaterialPool.MatFrom(def.uiIcon);
                var matrix = Matrix4x4.TRS(posShifted, Quaternion.identity, IconScale);
                Graphics.DrawMesh(MeshPool.plane14, matrix, iconMat, 0);
            }
        }
    }

    public virtual GlobalTargetInfo GetTargetForLine(ThrallControlGroup group) => group.Target;
}