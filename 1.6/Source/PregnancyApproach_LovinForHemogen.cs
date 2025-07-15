using RimWorld;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers
{
    public class PregnancyApproach_LovinForHemogen : PregnancyApproachWorker
    {
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
            if (target.HasActiveGene(GeneDefOf.Bloodfeeder))
            {
                if (biter.genes.HasActiveGene(RS_DefOf.VRE_SanguoFeeder) is false)
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
            target.needs.rest.CurLevelPercentage -= 0.5f;
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
