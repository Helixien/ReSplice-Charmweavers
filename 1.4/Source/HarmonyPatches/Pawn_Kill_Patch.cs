using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            if (__instance.Dead && __instance.RaceProps.Humanlike && __instance.relations != null)
            {
                foreach (var relation in __instance.relations.DirectRelations)
                {
                    if (relation.def == RS_DefOf.RX_Thrall)
                    {
                        var hediff = relation.otherPawn.health.hediffSet.GetFirstHediffOfDef(RS_DefOf.RX_LoveThrall) as Hediff_LoveThrall;
                        if (hediff != null)
                        {
                            hediff.MakeBerserk();
                            return;
                        }
                    }
                }
            }
        }
    }
}
