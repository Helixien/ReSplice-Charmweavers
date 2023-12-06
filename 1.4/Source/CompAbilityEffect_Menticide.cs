using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(Quest), nameof(Quest.End))]
    public static class test
    {
        public static void Postfix(Quest __instance)
        {
            Log.Message(__instance.name + " - ended");
        }
    }
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
            var gene = parent.pawn.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>();
            if (gene == null)
            {
                reason = "RS.CannotLovethrall".Translate();
                return true;
            }
            if (parent.pawn.GetThrallAmount() >= gene.LovethrallCapacity)
            {
                reason = "RS.AlreadyHasMaxThrallAmount".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            var hediff = HediffMaker.MakeHediff(RS_DefOf.RS_LoveThrall, target.Pawn) as Hediff_LoveThrall;
            hediff.master = parent.pawn;
            if (target.Pawn.HomeFaction != null && target.Pawn.HomeFaction != this.parent.pawn.Faction)
            {
                target.Pawn.HomeFaction.TryAffectGoodwillWith(parent.pawn.Faction,
                    Faction.OfPlayer.GoodwillToMakeHostile(target.Pawn.HomeFaction));
                hediff.previousFaction = target.Pawn.HomeFaction;
                QuestUtility.SendQuestTargetSignals(target.Pawn.HomeFaction.questTags, "BecameHostileToPlayer", target.Pawn.HomeFaction.Named("SUBJECT"));
                QuestUtility.SendQuestTargetSignals(target.Pawn.questTags, "ChangedFaction", this.Named("SUBJECT"), parent.pawn.Faction.Named("FACTION"));
                QuestUtility.SendQuestTargetSignals(target.Pawn.questTags, "ChangedFactionToPlayer", this.Named("SUBJECT"), parent.pawn.Faction.Named("FACTION"));
                QuestUtility.SendQuestTargetSignals(target.Pawn.questTags, "Arrested", target.Pawn.Named("SUBJECT"));
                if (target.Pawn.Faction != null)
                {
                    QuestUtility.SendQuestTargetSignals(target.Pawn.Faction.questTags, "FactionMemberArrested", target.Pawn.Faction.Named("FACTION"));
                }
                target.Pawn.GetLord()?.Notify_PawnAttemptArrested(target.Pawn);
                if (target.Pawn.Faction != this.parent.pawn.Faction)
                {
                    target.Pawn.SetFaction(this.parent.pawn.Faction, this.parent.pawn);
                }
                target.Pawn.guest.SetGuestStatus(null);
                foreach (var quest in Find.QuestManager.QuestsListForReading.SelectMany(x => x.PartsListForReading.OfType<QuestPart_ExtraFaction>()))
                {
                    quest.affectedPawns.Remove(target.Pawn);
                }
            }
            target.Pawn.health.AddHediff(hediff);
            target.Pawn.needs.mood.thoughts.memories.TryGainMemory(RS_DefOf.RS_BecameThrallMood, this.parent.pawn);

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
                    AbilityUtility.SendPostProcessedMessage("RS.TargetTooYoung".Translate(), targetPawn, ability);
                }
                return false;
            }
            return true;
        }

    }
}
