using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch("rjw.PregnancyHelper", "DoImpregnate")]
public static class RJW_VFEPregnancyApproach_DoImpregnate_Patch
{
    private static bool Prepare(MethodBase method) => method != null || ModsConfig.IsActive("rim.job.world");

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
    {
        var ourMethod = typeof(RJW_VFEPregnancyApproach_DoImpregnate_Patch).DeclaredMethod(nameof(ModifyPregnancyChance));
        var patchCount = 0;

        foreach (var ci in instr)
        {
            yield return ci;

            if (ci.opcode == OpCodes.Stloc_2)
            {
                // Load the first argument (pawn)
                yield return CodeInstruction.LoadArgument(0);
                // Load the second argument (partner)
                yield return CodeInstruction.LoadArgument(1);
                // Load the third local (pregnancy chance) as a reference
                yield return CodeInstruction.LoadLocal(2, true);
                // Call our custom method
                yield return new CodeInstruction(OpCodes.Call, ourMethod);

                patchCount++;
            }
        }

        const int expectedPatches = 1;
        if (patchCount != expectedPatches)
            Log.Error($"Patching RJW pregnancy chance failed - unexpected amount of patches. Expected patches: {expectedPatches}, actual patch amount: {patchCount}. Custom pregnancy approaches may not properly apply pregnancy chances.");
    }

    private static void ModifyPregnancyChance(Pawn pawn, Pawn partner, ref float currentChance)
    {
        // If not relations, return original chance
        if (pawn.relations == null || partner.relations == null)
            return;

        // Grab the current pregnancy approach and set the current chance to the static chance for partners (if present).
        // This will skip any sort of fertility checks and the like, just like how this works with vanilla code.
        var data = pawn.relations.GetAdditionalPregnancyApproachData();
        if (data?.partners != null && data.partners.TryGetValue(partner, out var def) && def.pregnancyChanceForPartners.HasValue)
            currentChance = def.pregnancyChanceForPartners.Value;
    }
}