using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{

    [HarmonyPatch(typeof(TransferableUIUtility), "DoExtraIcons")]
    public static class TransferableUIUtility_DoExtraIcons_Patch
    {
        private static float BondIconWidth = 24f;
        public static void Postfix(Transferable trad, Rect rect, ref float curX)
        {
            if (trad.AnyThing is Pawn pawn && pawn.IsLoveThrall(out var master))
            {
                var iconRect = new Rect(curX - BondIconWidth, (rect.height - BondIconWidth) / 2f, BondIconWidth, BondIconWidth);
                GUI.DrawTexture(iconRect, Core.LoveThrallIcon);
                if (Mouse.IsOver(iconRect))
                {
                    TooltipHandler.TipRegion(iconRect, "RX.ThrallOf".Translate(master.Named("PAWN")));
                }
                curX -= BondIconWidth;
            }
        }
    }
}
