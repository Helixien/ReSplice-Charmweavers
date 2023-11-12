using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ReSpliceCharmweaversSettings : ModSettings
    {
        public static int maxThrallAmount = 3;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref maxThrallAmount, "maxThrallAmount", 3);
        }

        private Vector2 scrollPos;
        private float scrollHeight = 99999999;

        public void DoSettingsWindowContents(Rect inRect)
        {
            var viewRect = new Rect(inRect.x, inRect.y, inRect.width - 16, scrollHeight);
            scrollHeight = 0;
            Widgets.BeginScrollView(inRect, ref scrollPos, viewRect);
            var ls = new Listing_Standard();
            ls.Begin(viewRect);
            var initY = ls.curY;
            maxThrallAmount = (int)ls.SliderLabeled("RS.MaxThrallAmount".Translate(), maxThrallAmount, 1, 10);
            ls.End();
            Widgets.EndScrollView();
            scrollHeight = ls.curY - initY;
        }
    }
}
