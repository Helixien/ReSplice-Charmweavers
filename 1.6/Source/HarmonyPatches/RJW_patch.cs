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
        public static FastInvokeHandler rjw_SexPropsSexTypeGetter;
        public static AccessTools.FieldRef<object, bool> rjw_SexPropsIsReverseField;

        public static bool Prepare(MethodBase method)
        {
            // Always succeed on every pass after the first.
            if (method != null)
                return true;

            var isActive = ModsConfig.IsActive("rim.job.world");
            if (isActive && rjw_SexPropsSexTypeGetter == null)
            {
                rjw_SexPropsSexTypeGetter = MethodInvoker.GetHandler(AccessTools.DeclaredPropertyGetter("rjw.SexProps:sexType"));
                rjw_SexPropsIsReverseField = AccessTools.FieldRefAccess<bool>("rjw.SexProps:isRevese");
            }

            return isActive;
        }

        public static MethodBase TargetMethod() => rjw_FinalizeThought ??= AccessTools.Method("rjw.AfterSexUtility:FinalizeThought");

        public static void Postfix(Pawn pawn, Pawn partner, bool isReceiving, object props)
        {
            if (pawn.relations.GetAdditionalPregnancyApproachData().partners.TryGetValue(partner, out var def))
            {
                if (props != null && def.Worker is PregnancyApproach_LovinForHemogen)
                {
                    // Get the rjwSextype enum and cast it to int.
                    var interaction = (int)rjw_SexPropsSexTypeGetter(props);

                    // The value of 16 is an oral interaction performed by both pawns, never drain hemogen.
                    if (interaction is 16)
                        return;

                    // The values 13 to 15 represent oral interactions performed by one pawn on another.
                    // If the pawn is performing the act, don't drain hemogen.
                    if (interaction is 13 or 14 or 15 && isReceiving != rjw_SexPropsIsReverseField(props))
                        return;
                }

                def.Worker.PostLovinEffect(pawn, partner);
            }
        }
    }
}
