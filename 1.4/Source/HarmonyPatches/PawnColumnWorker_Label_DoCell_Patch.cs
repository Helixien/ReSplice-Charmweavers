using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(PawnColumnWorker_Label), "DoCell")]
    public static class PawnColumnWorker_Label_DoCell_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var isLoveThrallInfo = AccessTools.Method(typeof(Core), nameof(Core.IsLovethrall), new Type[] {typeof(Pawn)});
            var get_IsSlaveInfo = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsSlave));
            var codes = codeInstructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (i > 0 && codes[i].opcode == OpCodes.Brtrue_S && codes[i - 1].Calls(get_IsSlaveInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call, isLoveThrallInfo);
                    yield return new CodeInstruction(OpCodes.Brtrue_S, codes[i].operand);
                }
            }
        }
    }
}
