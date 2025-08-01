using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{

    [HarmonyPatch(typeof(TransferableUIUtility), "DoExtraIcons")]
    public static class TransferableUIUtility_DoExtraIcons_Patch
    {
        private static float MasterIconWidth = 36f;
        public static void Postfix(Transferable trad, Rect rect, ref float curX)
        {
            if (trad.AnyThing is Pawn pawn && pawn.IsLovethrall(out var master))
            {
                var iconRect = new Rect(curX - MasterIconWidth, (rect.height - MasterIconWidth) / 2f, MasterIconWidth, MasterIconWidth);
                GUI.DrawTexture(iconRect, PortraitsCache.Get(master, new Vector2(MasterIconWidth, MasterIconWidth), Rot4.South));
                if (Mouse.IsOver(iconRect))
                {
                    Widgets.DrawHighlight(iconRect);
                    TooltipHandler.TipRegion(iconRect, "RS.ThrallOf".Translate(master.Named("PAWN")));
                }
                curX -= MasterIconWidth;
            }
        }
    }
}
