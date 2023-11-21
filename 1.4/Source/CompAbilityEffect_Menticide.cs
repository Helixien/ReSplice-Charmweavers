using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
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

        public override bool GizmoDisabled(out string reason)
        {
            if (parent.pawn.GetThrallAmount() >= ReSpliceCharmweaversSettings.maxThrallAmount)
            {
                reason = "RS.AlreadyHasMaxThrallAmount".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var hediff = HediffMaker.MakeHediff(RS_DefOf.RX_LoveThrall, target.Pawn) as Hediff_LoveThrall;
            hediff.master = parent.pawn;
            if (target.Pawn.Faction != this.parent.pawn.Faction)
            {
                hediff.previousFaction = target.Pawn.Faction;
                target.Pawn.SetFaction(this.parent.pawn.Faction, this.parent.pawn);
            }

            target.Pawn.health.AddHediff(hediff);
            target.Pawn.needs.mood.thoughts.memories.TryGainMemory(RS_DefOf.RX_BecameThrallMood, this.parent.pawn);

            foreach (Pawn p in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
            {
                if (p.needs.mood != null && p != target.Pawn)
                {
                    p.needs.mood.thoughts.memories.TryGainMemory(RS_DefOf.RS_EnthralledPrisoner, target.Pawn);
                }
            }
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return false;
            }
            if (pawn.IsLoveThrall(out var master) && master == parent.pawn)
            {
                return false;
            }
            if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) <= 0)
            {
                if (throwMessages)
                {
                    Messages.Message(parent.def.LabelCap + ": " + "AbilityTargetPsychicallyDeaf".Translate(), target.ToTargetInfo(pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
                }
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
            return base.Valid(target, throwMessages);
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
