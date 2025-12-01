using System.Reflection;
using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch]
public static class VRE_Highmate_Breakup_Patch
{
    public static bool Prepare(MethodBase method)
    {
        if (method != null)
            return true;
        if (!ModLister.AnyModActiveNoSuffix(["vanillaracesexpanded.highmate"]))
            return false;
        if (TargetMethod() != null)
            return true;

        Log.Error("Patching VRE-Highmate/RJW compat failed - could not find method \"VanillaRacesExpandedHighmate.JobDriver_InitiateLovin:ProcessBreakups\"");
        return false;
    }

    private static MethodBase TargetMethod() => AccessTools.DeclaredMethod("VanillaRacesExpandedHighmate.JobDriver_InitiateLovin:ProcessBreakups");

    // Don't force breakups between a master and their thrall
    public static bool Prefix(Pawn initiator, Pawn recipient)
        => !initiator.relations.DirectRelationExists(RS_DefOf.RS_Master, recipient) && !initiator.relations.DirectRelationExists(RS_DefOf.RS_Thrall, recipient);
}