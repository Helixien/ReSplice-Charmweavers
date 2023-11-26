using HarmonyLib;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            if (__instance.Dead)
            {
                FreeAllThrals(__instance);
            }
        }

        public static void FreeAllThrals(Pawn __instance)
        {
            if (__instance.RaceProps.Humanlike && __instance.relations != null)
            {
                foreach (var relation in __instance.relations.DirectRelations.ToList())
                {
                    if (relation.def == RS_DefOf.RS_Thrall)
                    {
                        var hediff = relation.otherPawn.health.hediffSet.GetFirstHediffOfDef(RS_DefOf.RS_LoveThrall) as Hediff_LoveThrall;
                        if (hediff != null)
                        {
                            hediff.pawn.health.RemoveHediff(hediff);
                        }
                    }
                }
            }
        }
    }
}
