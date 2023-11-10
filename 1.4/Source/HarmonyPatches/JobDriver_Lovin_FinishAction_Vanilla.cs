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
            if (jobDriver.pawn.genes?.HasGene(RS_DefOf.RS_LoveFeed) ?? false)
            {
                DoLoveFeed(jobDriver.pawn, jobDriver.TargetA.Pawn);
            }
            if (jobDriver.TargetA.Pawn.genes?.HasGene(RS_DefOf.RS_LoveFeed) ?? false)
            {
                DoLoveFeed(jobDriver.TargetA.Pawn, jobDriver.pawn);
            }
        }

        private static void DoLoveFeed(Pawn biter, Pawn target)
        {
            if (target.genes?.HasGene(GeneDefOf.Bloodfeeder) ?? false)
            {
                if (biter.genes.HasGene(RS_DefOf.VRE_SanguoFeeder) is false)
                {
                    return;
                }
            }
            float num = BloodlossAfterBite(target);
            if (num >= HediffDefOf.BloodLoss.lethalSeverity)
            {
                return;
            }
            else if (HediffDefOf.BloodLoss.stages[HediffDefOf.BloodLoss.StageAtSeverity(num)].lifeThreatening)
            {
                return;
            }
            SanguophageUtility.DoBite(biter, target, 0.2f, 0.1f, 0.4499f / 2f, 1f,
                IntRange.one, ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
        }

        private static float BloodlossAfterBite(Pawn target)
        {
            if (target.Dead || !target.RaceProps.IsFlesh)
            {
                return 0f;
            }
            float num = 0.4499f / 2f;
            Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (firstHediffOfDef != null)
            {
                num += firstHediffOfDef.Severity;
            }
            return num;
        }
    }
}
