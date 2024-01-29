using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), "AddDirectRelation")]
    public static class Pawn_RelationsTracker_AddDirectRelation_Patch
    {
        public static void Postfix(Pawn ___pawn, PawnRelationDef def, Pawn otherPawn)
        {
            if (def == PawnRelationDefOf.ExSpouse)
            {
                if (___pawn.Ideo != null)
                {
                    foreach (var precept in ___pawn.Ideo.PreceptsListForReading)
                    {
                        foreach (var comp in precept.def.comps)
                        {
                            if (comp is PreceptComp_Divorce preceptDivorce)
                            {
                                preceptDivorce.DoDivorceThought(___pawn, otherPawn, precept);
                            }
                        }
                    }
                }
            }
        }
    }
}
