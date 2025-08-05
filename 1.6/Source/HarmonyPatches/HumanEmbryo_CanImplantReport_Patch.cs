using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(HumanEmbryo), nameof(HumanEmbryo.CanImplantReport))]
public static class HumanEmbryo_CanImplantReport_Patch
{
    private static void Postfix(Pawn pawn, ref AcceptanceReport __result)
    {
        // Not accepted, don't change anything and display current message
        if (!__result.Accepted)
            return;

        // Don't allow if at 3 pregnancies or recently impregnated
        if (pawn.genes?.HasActiveGene(RS_DefOf.RS_MultiPregnancy) == true)
        {
            if (pawn.health.hediffSet.hediffs.Count(x => x.def == HediffDefOf.PregnantHuman) >= ReSpliceCharmweaversSettings.maxMultiPregnancyAmount)
                __result = "RS.CannotTooPregnant".Translate();
            else if (pawn.health.hediffSet.HasHediff(RS_DefOf.RS_RecentImpregnation))
                __result = "RS.CannotPregnantTooRecently".Translate();
        }
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var targetMethod = typeof(HediffSet).DeclaredMethod(nameof(HediffSet.HasHediff), [typeof(HediffDef), typeof(bool)]);
        var ourMethod = typeof(HumanEmbryo_CanImplantReport_Patch).DeclaredMethod(nameof(IsMultiPregnancyDisallowed));

        var callsInserted = 0;

        foreach (var instr in instructions)
        {
            yield return instr;

            // Baiscally, surround 'if (HasHediff())' call with 'if (IsMultiPregnancyDisallowed(HasHediff()), pawn)'
            if (instr.Calls(targetMethod))
            {
                yield return CodeInstruction.LoadArgument(1);
                yield return new CodeInstruction(OpCodes.Call, ourMethod);

                callsInserted++;
            }
        }

        const int expectedCalls = 1;
        if (callsInserted != expectedCalls)
            Log.Error($"Patched incorrect amount of calls to HediffSet.HasHediff. Expected: {expectedCalls}, actual: {callsInserted}.");
    }

    private static bool IsMultiPregnancyDisallowed(bool result, Pawn pawn)
    {
        // Check result first to avoid a more costly second check, which may not be needed
        return result && pawn.genes?.HasActiveGene(RS_DefOf.RS_MultiPregnancy) != true;
    }
}