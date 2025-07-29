using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch("VanillaRacesExpandedHighmate.CompAbilityEffect_InitiateLovin", "Apply")]
[HarmonyPatch([typeof(LocalTargetInfo), typeof(LocalTargetInfo)])]
public static class VRE_Highmate_RJW_InitiateLoving_Patch
{
    public static bool Prepare(MethodBase method) => method != null || (ModsConfig.IsActive("rim.job.world") && ModsConfig.IsActive("vanillaracesexpanded.highmate"));

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