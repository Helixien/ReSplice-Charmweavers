using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(TransferableOneWayWidget), "DoRow")]
    public static class TransferableOneWayWidget_DoRow_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.opcode == OpCodes.Stloc_S && code.operand is LocalBuilder lb && lb.LocalType == typeof(Color))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, lb.LocalIndex);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TransferableOneWayWidget_DoRow_Patch), nameof(TrySetColor)));
                }
            }
        }

        public static void TrySetColor(Pawn pawn, ref Color color)
        {
            if (pawn.IsLoveThrall())
            {
                color = Core.ThrallColor;
            }
        }
    }
}
