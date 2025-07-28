using RimWorld;
using VEF.Pawns;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers
{
    public class PregnancyApproach_LovinForHemogen : PregnancyApproachWorker
    {
        private const float LoveBiteBloodLoss = JobDriver_PrisonerBloodfeed.BloodLoss / 2f;
        private const float HemogenGain = JobDriver_PrisonerBloodfeed.HemogenGain / 2f;
        private const float NutritionGain = JobDriver_PrisonerBloodfeed.NutritionGain / 2f;

        public override void ModifyLovinToil(Toil lovinToil, Pawn pawn, Pawn partner)
        {
            base.ModifyLovinToil(lovinToil, pawn, partner);
            if (GetSafeHemogenAmountPercentage(pawn, partner) > 0.01f)
                lovinToil.WithEffect(RS_DefOf.Bloodfeed_Warmup, TargetIndex.A);
        }

        public override void PostLovinEffect(Pawn pawn, Pawn partner)
        {
            base.PostLovinEffect(pawn, partner);
            DoLovinResult(pawn, partner);
        }

        public static void DoLovinResult(Pawn pawn, Pawn partner)
        {
            DoLoveFeed(pawn, partner);
        }

        private static void DoLoveFeed(Pawn biter, Pawn target)
        {
            var percent = GetSafeHemogenAmountPercentage(biter, target);
            if (percent <= 0.01f)
                return;

            SanguophageUtility.DoBite(biter, target, HemogenGain * percent, NutritionGain * percent, LoveBiteBloodLoss * percent, 1f,
                IntRange.One, ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
            target.needs.rest.CurLevelPercentage -= 0.5f * percent;
        }

        private static float GetSafeHemogenAmountPercentage(Pawn biter, Pawn target)
        {
            if (!biter.HasActiveGene(RS_DefOf.RS_LoveFeed))
                return 0;

            var hemogen = biter.genes.GetFirstGeneOfType<Gene_Hemogen>();
            if (hemogen == null || hemogen.ValuePercent > 0.9f)
                return 0;

            if (target.HasActiveGene(GeneDefOf.Bloodfeeder) && biter.genes.HasActiveGene(RS_DefOf.VRE_SanguoFeeder) is false)
                return 0;

            if (target.Dead || !target.RaceProps.IsFlesh)
                return 0;

            var bloodLossHediff = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            // No blood loss, allow full amount
            if (bloodLossHediff == null)
                return 1;

            var maxBloodLoss = HediffDefOf.BloodLoss.lethalSeverity;
            var lifeThreateningStage = HediffDefOf.BloodLoss.stages.FirstOrDefault(x => x.lifeThreatening);
            if (lifeThreateningStage != null && lifeThreateningStage.minSeverity < maxBloodLoss)
                maxBloodLoss = lifeThreateningStage.minSeverity;
            // Just a small precaution in case it would exactly end on the disallowed amount
            maxBloodLoss -= 0.001f;

            // Hediff is already over max blood loss, not allowed at all
            if (bloodLossHediff.Severity >= maxBloodLoss)
                return 0;
            // Under max blood loss, allow full amount
            if (bloodLossHediff.Severity + LoveBiteBloodLoss < maxBloodLoss)
                return 1;

            // Under allowed blood loss but would cause medical emergency, scale amount to be right under max
            return (maxBloodLoss - bloodLossHediff.Severity) / LoveBiteBloodLoss;
        }
    }
}
