using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch]
    public static class VRE_Highmate_RJW_patch
    {
        public static bool Prepare() => RJW_patch.Prepare() && ModsConfig.IsActive("vanillaracesexpanded.highmate");
        public static MethodBase TargetMethod() => RJW_patch.TargetMethod();

        public static void Postfix(ThoughtDef thoughtgain, Pawn pawn)
        {
            VanillaRacesExpandedHighmate.Need_Lovin need = pawn?.needs?.TryGetNeed<VanillaRacesExpandedHighmate.Need_Lovin>();
            if (need != null)
            {
                need.CurLevel = 1f;
            }
        }
    }
}
