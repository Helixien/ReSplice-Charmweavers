using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch]
public static class NoUnnaturalCorpseForPawns_Patch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        // We could technically just patch GameComponent_Anomaly:PawnHasUnnaturalCorpse.
        // However, this feels like a quick path towards causing bugs due to mod incompatibilities.
        yield return typeof(IncidentWorker_UnnaturalCorpseArrival).DeclaredMethod("ValidatePawn");
        yield return typeof(QuestNode_Root_MysteriousCargoUnnaturalCorpse).DeclaredMethod("ValidatePawn");
    }

    private static void Postfix(Pawn pawn, ref bool __result)
    {
        if (__result && pawn.IsLovethrall())
            __result = false;
    }
}