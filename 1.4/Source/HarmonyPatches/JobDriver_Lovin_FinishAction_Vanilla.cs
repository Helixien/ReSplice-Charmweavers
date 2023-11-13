using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch]
    static class JobDriver_Lovin_FinishAction_Vanilla
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.GetDeclaredMethods(typeof(JobDriver_Lovin))
                .LastOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.opcode == OpCodes.Stloc_0 && codes[i - 3].LoadsField(AccessTools.Field(typeof(ThoughtDefOf), "GotSomeLovin")))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JobDriver_Lovin_FinishAction_Vanilla), nameof(DoLovinResult)));
                }
            }
        }

        public static void DoLovinResult(JobDriver_Lovin jobDriver, ref Thought_Memory thoughtDef)
        {
            var thrallHediff = jobDriver.pawn.health.hediffSet.GetFirstHediffOfDef(RS_DefOf.RX_LoveThrall) as Hediff_LoveThrall;
            if (thrallHediff != null && jobDriver.TargetA.Pawn == thrallHediff.master)
            {
                thoughtDef = (Thought_Memory)ThoughtMaker.MakeThought(RS_DefOf.RX_GotSomeLovinThrall);
            }
        }
    }
}
