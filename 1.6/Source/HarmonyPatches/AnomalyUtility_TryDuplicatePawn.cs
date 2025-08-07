using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(AnomalyUtility), nameof(AnomalyUtility.TryDuplicatePawn))]
public static class AnomalyUtility_TryDuplicatePawn
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var callsInserted = 0;

        foreach (var instr in instructions)
        {
            if (instr.opcode == OpCodes.Switch)
            {
                // Load the originalPawn argument.
                yield return CodeInstruction.LoadArgument(0);
                // The current instruction should be Ldloc_0, so we wrap it with our method.
                yield return new CodeInstruction(OpCodes.Call, typeof(AnomalyUtility_TryDuplicatePawn).DeclaredMethod(nameof(ModifyIndex)));
                // Set the result back.
                yield return CodeInstruction.StoreLocal(0);
                // And load it again.
                yield return CodeInstruction.LoadLocal(0);
                // We could skip the last 2 parts, but if we return a number bigger than 3 or smaller than 0
                // it would cause the error to report that whichever the number it was originally is incorrect.

                callsInserted++;
            }

            yield return instr;
        }

        const int expectedCalls = 1;
        if (callsInserted != expectedCalls)
            Log.Error($"Patched incorrect amount of switch calls inside AnomalyUtility:TryDuplicatePawn method. Expected: {expectedCalls}, actual: {callsInserted}.");
    }

    private static int ModifyIndex(int index, Pawn pawn)
    {
        // If index 3 (aggressive outcome) and the pawn is lovethrall, set the index to 0 (normal outcome).
        // Outcomes 1/2 could potentially be cancelled if the original pawn has organ decay/crumbling mind.
        if (index == 3 && pawn != null && pawn.IsLovethrall())
            return 0;
        return index;
    }
}