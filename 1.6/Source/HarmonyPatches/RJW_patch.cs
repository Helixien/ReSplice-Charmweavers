using HarmonyLib;
using System.Reflection;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch]
    public static class RJW_patch
    {
        public static MethodBase rjw_FinalizeThought;
        public static bool Prepare() => ModsConfig.IsActive("rim.job.world");
        public static MethodBase TargetMethod() => rjw_FinalizeThought ??= AccessTools.Method("rjw.AfterSexUtility:FinalizeThought");

        public static void Postfix(Pawn pawn, Pawn partner)
        {
            if (pawn.relations.GetAdditionalPregnancyApproachData().partners.TryGetValue(partner, out var def))
            {
                def.Worker.PostLovinEffect(pawn, partner);
            }
        }
    }
}
