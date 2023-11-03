using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(PregnancyUtility), "ApplyBirthOutcome")]
    public static class PregnancyUtility_ApplyBirthOutcome_Patch
    {
        public static Pawn mother;
        public static void Prefix(OutcomeChance outcome, float quality, Precept_Ritual ritual, List<GeneDef> genes,
            Pawn geneticMother, Thing birtherThing, Pawn father = null, Pawn doctor = null, LordJob_Ritual lordJobRitual = null, RitualRoleAssignments assignments = null)
        {
            mother = geneticMother;
        }

        public static void Postfix()
        {
            mother = null;
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        public static void Prefix(ref PawnGenerationRequest request)
        {
            if (PregnancyUtility_ApplyBirthOutcome_Patch.mother != null)
            {
                var gene = PregnancyUtility_ApplyBirthOutcome_Patch.mother.genes.GetGene(RS_DefOf.RS_Matrianic);
                if (gene != null && Rand.Chance(0.9f))
                {
                    request.FixedGender = Gender.Female;
                }
                PregnancyUtility_ApplyBirthOutcome_Patch.mother = null;
            }
        }
    }

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

    [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight")]
    public static class InteractionWorker_RomanceAttempt_RandomSelectionWeight_Patch
    {
        public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.IsLoveThrall())
            {
                __result = 0f;
            }
            else if (recipient.IsLoveThrall())
            {
                __result = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(InteractionWorker_Breakup), "RandomSelectionWeight")]
    public static class InteractionWorker_Breakup_RandomSelectionWeight_Patch
    {
        public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.IsLoveThrall(out var master) && recipient == master)
            {
                __result = 0f;
            }
            else if (recipient.IsLoveThrall(out var master2) && initiator == master2)
            {
                __result = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(PawnNameColorUtility), "PawnNameColorOf")]
    public static class PawnNameColorUtility_PawnNameColorOf_Patch
    {
        public static void Postfix(ref Color __result, Pawn pawn)
        {
            if (pawn.IsLoveThrall())
            {
                __result = Core.SuccubColor;
            }
        }
    }

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
            if (pawn.IsLoveThrall(out var master))
            {
                ColonistBarColonistDrawer.tmpIconsToDraw.Add(new ColonistBarColonistDrawer.IconDrawCall(Core.LoveThrallIcon, "RX.ThrallOf".Translate(master.Named("PAWN"))));
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            if (__instance.Dead && __instance.RaceProps.Humanlike && __instance.relations != null)
            {
                foreach (var relation in __instance.relations.DirectRelations)
                {
                    if (relation.def == RS_DefOf.RX_Thrall)
                    {
                        var hediff = relation.otherPawn.health.hediffSet.GetFirstHediffOfDef(RS_DefOf.RX_LoveThrall) as Hediff_LoveThrall;
                        if (hediff != null)
                        {
                            hediff.MakeBerserk();
                            return;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnColumnWorker_Label), "DoCell")]
    public static class PawnColumnWorker_Label_DoCell_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var isLoveThrallInfo = AccessTools.Method(typeof(Core), nameof(Core.IsLoveThrall), new Type[] {typeof(Pawn)});
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

    [HarmonyPatch(typeof(TransferableUIUtility), "DoExtraIcons")]
    public static class TransferableUIUtility_DoExtraIcons_Patch
    {
        private static float BondIconWidth = 24f;
        public static void Postfix(Transferable trad, Rect rect, ref float curX)
        {
            if (trad.AnyThing is Pawn pawn && pawn.IsLoveThrall(out var master))
            {
                var iconRect = new Rect(curX - BondIconWidth, (rect.height - BondIconWidth) / 2f, BondIconWidth, BondIconWidth);
                GUI.DrawTexture(iconRect, Core.LoveThrallIcon);
                if (Mouse.IsOver(iconRect))
                {
                    TooltipHandler.TipRegion(iconRect, "RX.ThrallOf".Translate(master.Named("PAWN")));
                }
                curX -= BondIconWidth;
            }
        }
    }
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
                color = Core.SuccubColor;
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryRomanceChanceFactor")]
    public static class Pawn_RelationsTracker_SecondaryRomanceChanceFactor_Patch
    {
        public static void Postfix(ref float __result, Pawn_RelationsTracker __instance, Pawn otherPawn)
        {
            if (__instance.pawn.genes?.HasGene(RS_DefOf.RS_Beauty_Angelic) ?? false)
            {
                __result *= 1.6f;
            }
        }
    }

    [HarmonyPatch(typeof(LovePartnerRelationUtility), "GetLovinMtbHours")]
    public static class LovePartnerRelationUtility_GetLovinMtbHours_Patch
    {
        public static void Postfix(ref float __result, Pawn pawn, Pawn partner)
        {
            if ((pawn.genes?.HasGene(RS_DefOf.RS_Libido_None) ?? false) 
                || (partner.genes?.HasGene(RS_DefOf.RS_Libido_None) ?? false))
            {
                __result = -1f;
            }
        }
    }

    [HarmonyPatch(typeof(JobGiver_DoLovin), "TryGiveJob")]
    public static class JobGiver_DoLovin_TryGiveJob_Patch
    {
        public static void Postfix(ref Job __result, Pawn pawn)
        {
            if (pawn.genes?.HasGene(RS_DefOf.RS_Libido_None) ?? false)
            {
                __result = null;
            }
            if (__result != null)
            {
                var target = __result.targetA.Pawn;
                if (target != null && (target.genes?.HasGene(RS_DefOf.RS_Libido_None) ?? false)) 
                {
                    __result = null;
                }
            }
        }
    }
}
