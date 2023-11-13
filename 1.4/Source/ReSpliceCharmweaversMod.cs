using HarmonyLib;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ReSpliceCharmweaversMod : Mod
    {
        public static ReSpliceCharmweaversSettings settings;
        public ReSpliceCharmweaversMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<ReSpliceCharmweaversSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }
}
