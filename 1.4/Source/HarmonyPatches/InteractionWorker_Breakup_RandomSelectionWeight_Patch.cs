using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(InteractionWorker_Breakup), "RandomSelectionWeight")]
    public static class InteractionWorker_Breakup_RandomSelectionWeight_Patch
    {
        public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.IsLoveThrall(out var master) && recipient == master)
            {
                __result = 0f;
            }
            else if (recipient.IsLoveThrall(out var master2) && initiator == master2)
            {
                __result = 0f;
            }

            if (initiator.HasPrecept(RS_DefOf.RS_Divorce_Forbidden))
            {
                __result = 0f;
            }
            if (initiator.gender == Gender.Male && initiator.HasPrecept(RS_DefOf.RS_Divorce_FemaleOnly))
            {
                __result = 0f;
            }
            if (initiator.gender == Gender.Female && initiator.HasPrecept(RS_DefOf.RS_Divorce_MaleOnly))
            {
                __result = 0f;
            }
        }

        public static bool HasPrecept(this Pawn pawn, PreceptDef precept)
        {
            return pawn?.ideo?.Ideo?.HasPrecept(precept) ?? false;
        }
    }
}
