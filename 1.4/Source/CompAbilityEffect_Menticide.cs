using RimWorld;
using Verse;

namespace RareXenotypesSuccubus
{
    public class CompProperties_Menticide : CompProperties_AbilityEffect
    {
        public CompProperties_Menticide()
        {
            compClass = typeof(CompAbilityEffect_Menticide);
        }
    }
    public class CompAbilityEffect_Menticide : CompAbilityEffect
    {
        public override bool HideTargetPawnTooltip => true;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var hediff = HediffMaker.MakeHediff(RX_DefOf.RX_LoveThrall, target.Pawn) as Hediff_LoveThrall;
            hediff.master = parent.pawn;
            if (target.Pawn.Faction != this.parent.pawn.Faction)
            {
                hediff.previousFaction = target.Pawn.Faction;
                target.Pawn.SetFaction(this.parent.pawn.Faction, this.parent.pawn);
            }

            target.Pawn.health.AddHediff(hediff);
            target.Pawn.needs.mood.thoughts.memories.TryGainMemory(RX_DefOf.RX_BecameThrallMood, this.parent.pawn);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return false;
            }
            if (!AbilityUtility.ValidateMustBeHuman(pawn, throwMessages, parent))
            {
                return false;
            }
            if (!ValidateMustNotBeChild(pawn, throwMessages, parent))
            {
                return false;
            }
            return true;
        }

        public static bool ValidateMustNotBeChild(Pawn targetPawn, bool showMessages, Ability ability)
        {
            if (targetPawn.DevelopmentalStage.Juvenile())
            {
                if (showMessages)
                {
                    AbilityUtility.SendPostProcessedMessage("RX.TargetTooYoung".Translate(), targetPawn, ability);
                }
                return false;
            }
            return true;
        }

    }
}
