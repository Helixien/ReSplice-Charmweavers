using RimWorld;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers
{
    public class PregnancyApproach_LovinForHemogen : PregnancyApproachWorker
    {
        private const float LoveBiteBloodLoss = JobDriver_PrisonerBloodfeed.BloodLoss / 2f;
        private const float HemogenGain = JobDriver_PrisonerBloodfeed.HemogenGain / 2f;
        private const float NutritionGain = JobDriver_PrisonerBloodfeed.NutritionGain / 2f;

        public override void PostLovinEffect(Pawn pawn, Pawn partner)
        {
            base.PostLovinEffect(pawn, partner);
            DoLovinResult(pawn, partner);
        }

        public static void DoLovinResult(Pawn pawn, Pawn partner)
        {
            if (pawn.HasActiveGene(RS_DefOf.RS_LoveFeed))
            {
                DoLoveFeed(pawn, partner);
            }
        }

        private static void DoLoveFeed(Pawn biter, Pawn target)
        {
            var hemogen = biter.genes.GetFirstGeneOfType<Gene_Hemogen>();
            if (hemogen == null || hemogen.ValuePercent > 0.9f)
            {
                return;
            }

            if (target.HasActiveGene(GeneDefOf.Bloodfeeder))
            {
                if (biter.genes.HasActiveGene(RS_DefOf.VRE_SanguoFeeder) is false)
                {
                    return;
                }
            }

            var percent = GetSafeHemogenAmountPercentage(target);
            if (percent <= 0.01f)
            {
                return;
            }

            SanguophageUtility.DoBite(biter, target, HemogenGain * percent, NutritionGain * percent, LoveBiteBloodLoss * percent, 1f,
                IntRange.One, ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
            target.needs.rest.CurLevelPercentage -= 0.5f * percent;
        }

        private static float GetSafeHemogenAmountPercentage(Pawn target)
        {
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
