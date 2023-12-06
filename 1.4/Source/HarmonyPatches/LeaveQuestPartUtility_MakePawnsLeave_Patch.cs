using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(LeaveQuestPartUtility), "MakePawnsLeave")]
    public static class LeaveQuestPartUtility_MakePawnsLeave_Patch
    {
        public static void Prefix(ref IEnumerable<Pawn> pawns)
        {
            pawns = pawns.Where(x => x.IsLoveThrall(out var master) is false || master.Faction != Faction.OfPlayer).ToList();
        }
    }
}
