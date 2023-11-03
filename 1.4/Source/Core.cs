using HarmonyLib;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [StaticConstructorOnStartup]
    public static class Core
    {
        public static bool VRESangouphageActive = ModsConfig.IsActive("vanillaracesexpanded.sanguophage");
        public static Texture2D LoveThrallIcon = ContentFinder<Texture2D>.Get("UI/Icons/Thrall");
        static Core()
        {
            new Harmony("ReSpliceCharmweavers.Mod").PatchAll();
        }

        public static Color SuccubColor = new ColorInt(198, 122, 211).ToColor;
        public static bool IsLoveThrall(this Pawn pawn)
        {
            return IsLoveThrall(pawn, out _);
        }

        public static bool IsLoveThrall(this Pawn pawn, out Pawn master)
        {
            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(RS_DefOf.RX_LoveThrall) as Hediff_LoveThrall;
            if (hediff != null)
            {
                master = hediff.master;
                return true;
            }
            else
            {
                master = null;
                return false;
            }
        }
    }
}
