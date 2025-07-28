using System.Reflection;
using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch("VanillaRacesExpandedHighmate.JobDriver_InitiateLovin", "ProcessBreakups")]
public static class VRE_Highmate_Breakup_Patch
{
    public static bool Prepare(MethodBase method) => method != null || ModsConfig.IsActive("vanillaracesexpanded.highmate");

    // Don't force breakups between a master and their thrall
    public static bool Prefix(Pawn initiator, Pawn recipient)
        => !initiator.relations.DirectRelationExists(RS_DefOf.RS_Master, recipient) && !initiator.relations.DirectRelationExists(RS_DefOf.RS_Thrall, recipient);
}