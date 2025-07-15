using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch]
    static class JobDriver_Lovin_FinishAction_VSIE
    {
        [HarmonyPrepare]
        public static bool Prepare()
        {
            FindMethod();
            return methodTarget != null;
        }

        private static void FindMethod()
        {
            var type = AccessTools.TypeByName("VanillaSocialInteractionsExpanded.JobDriver_LovinOneNightStand");
            if (type != null)
            {
                methodTarget = AccessTools.GetDeclaredMethods(type).LastOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
            }
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod() => methodTarget;

        public static MethodInfo methodTarget;
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var type = AccessTools.TypeByName("VanillaSocialInteractionsExpanded.VSIE_DefOf");
            var field = AccessTools.Field(type, "VSIE_GotSomeLovin");
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (code.opcode == OpCodes.Stloc_0 && codes[i - 3].LoadsField(field))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JobDriver_Lovin_FinishAction_Vanilla),
                        nameof(JobDriver_Lovin_FinishAction_Vanilla.DoLovinResult)));
                }
            }
        }
    }
}
