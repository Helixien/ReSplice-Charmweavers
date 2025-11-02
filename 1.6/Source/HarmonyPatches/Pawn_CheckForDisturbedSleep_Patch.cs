using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.CheckForDisturbedSleep))]
[HarmonyPatchCategory(Core.LatePatchesCategory)]
public static class Pawn_CheckForDisturbedSleep_Patch
{
    // Co-spouses require Ideology, so check if the def exists
    private static bool Prepare(MethodBase method) => method != null || RS_DefOf.RS_CoSpouse != null;

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr, ILGenerator generator)
    {
        var targetMethod = typeof(LovePartnerRelationUtility).DeclaredMethod(nameof(LovePartnerRelationUtility.LovePartnerRelationExists));
        var ourMethod = typeof(Pawn_CheckForDisturbedSleep_Patch).DeclaredMethod(nameof(IsCoSpouse));

        var matcher = new CodeMatcher(instr, generator);

        // Match from start, set position to beginning of the sequence.
        // Look for `if (LovePartnerRelationUtility.LovePartnerRelationExists(this, source)) return;`
        matcher.MatchStartForward(
            CodeMatch.IsLdarg(),
            CodeMatch.IsLdarg(),
            CodeMatch.Calls(targetMethod),
            CodeMatch.Branches(),
            new CodeMatch(OpCodes.Ret)
        );

        // Grab the argument index of the firs pawn
        var firstPawnIndex = matcher.Instruction.ArgumentIndex();
        // Move forward and grab the second index
        matcher.Advance(1);
        var secondPawnIndex = matcher.Instruction.ArgumentIndex();
        // Move back again
        matcher.Advance(-1);
        // Make a label on the current instruction
        matcher.CreateLabel(out var label);
        // Insert our code right before the current instruction
        matcher.Insert(
            // Load in the 2 pawns
            CodeInstruction.LoadArgument(firstPawnIndex),
            CodeInstruction.LoadArgument(secondPawnIndex),
            // Call our method to check if they are co-spouses
            new CodeInstruction(OpCodes.Call, ourMethod),
            // Jump past our ret instruction if there's no co-spouse relation between the pawns
            new CodeInstruction(OpCodes.Brfalse_S, label),
            // Return if there is a co-spouse relation, preventing the disturbed sleep
            new CodeInstruction(OpCodes.Ret)
        );

        return matcher.Instructions();
    }

    private static bool IsCoSpouse(Pawn pawn, Pawn other) => pawn.relations.DirectRelationExists(RS_DefOf.RS_CoSpouse, other);
}