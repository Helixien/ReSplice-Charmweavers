using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch]
public static class VRE_Highmate_RJW_InitiateLoving_Patch
{
    public static bool Prepare(MethodBase method)
    {
        if (method != null) return true;
        if (!ModLister.AllModsActiveNoSuffix(["rim.job.world", "vanillaracesexpanded.highmate"]))
            return false;
        if (TargetMethod() != null)
            return true;

        Log.Error("Patching VRE-Highmate/RJW compat failed - could not find method \"VanillaRacesExpandedHighmate.CompAbilityEffect_InitiateLovin:Apply\"");
        return false;
    }

    private static MethodBase TargetMethod() => AccessTools.DeclaredMethod("VanillaRacesExpandedHighmate.CompAbilityEffect_InitiateLovin:Apply", [typeof(LocalTargetInfo), typeof(LocalTargetInfo)]);

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        // Replace the VRE job with the one from RJW, as the VRE one does not interact with RJW systems.
        var targetField = AccessTools.DeclaredField("VanillaRacesExpandedHighmate.InternalDefOf:VRE_InitiateLovin");
        var replacementField = AccessTools.DeclaredField("rjw.xxx:quick_sex");

        foreach (var instr in instructions)
        {
            if (instr.LoadsField(targetField))
                instr.operand = replacementField;

            yield return instr;
        }
    }
}