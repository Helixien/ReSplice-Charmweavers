using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(ColonistBarColonistDrawer), "DrawIcons")]
    public static class ColonistBarColonistDrawer_DrawIcons_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.operand is MethodInfo info && info.ToString().Contains("Void Clear()"))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ColonistBarColonistDrawer_DrawIcons_Patch),
                        nameof(LoadIconIfThrall)));
                }
            }
        }

        public static void LoadIconIfThrall(Pawn pawn)
        {
            if (pawn.IsLovethrall(out var master))
            {
                ColonistBarColonistDrawer.tmpIconsToDraw.Add(new ColonistBarColonistDrawer.IconDrawCall(Core.LoveThrallIcon, "RS.ThrallOf".Translate(master.Named("PAWN"))));
            }
        }
    }
}
