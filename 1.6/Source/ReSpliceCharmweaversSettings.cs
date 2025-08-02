using RimWorld;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ReSpliceCharmweaversSettings : ModSettings
    {
        public static int maxThrallAmount = 3;
        public static int maxThrallControlGroupAmount = 2;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref maxThrallAmount, "maxThrallAmount", 3);
            Scribe_Values.Look(ref maxThrallControlGroupAmount, "maxThrallControlGroupAmount", 2);
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
            maxThrallAmount = (int)ls.SliderLabeled("RS.MaxThrallAmount".Translate() + ": " + maxThrallAmount, maxThrallAmount, 1, 10);
            var previous = maxThrallControlGroupAmount;
            maxThrallControlGroupAmount = (int)ls.SliderLabeled("RS.MaxThrallControlGroupAmount".Translate() + ": " + maxThrallControlGroupAmount, maxThrallControlGroupAmount, 1, 10);
            if (maxThrallControlGroupAmount != previous && Current.ProgramState == ProgramState.Playing && Current.Game != null)
            {
                foreach (var pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
                    pawn.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>()?.Notify_ControlGroupAmountMayChanged();
            }
            ls.End();
            Widgets.EndScrollView();
            scrollHeight = ls.curY - initY;
        }
    }
}
