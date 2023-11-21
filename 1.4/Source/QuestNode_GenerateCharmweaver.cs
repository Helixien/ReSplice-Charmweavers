using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    public class QuestNode_GenerateCharmweaver : QuestNode
    {
        public SlateRef<string> storeAs;
        public SlateRef<PawnKindDef> kindDef;
        public override void RunInt()
        {
            Slate slate = QuestGen.slate;
            PawnGenerationRequest request = new PawnGenerationRequest(fixedGender: Rand.Chance(0.9f) ? Gender.Female 
                : Gender.Male, kind: kindDef.GetValue(slate), forcedXenotype: RS_DefOf.RS_Charmweaver, 
                forcedCustomXenotype: null, allowedXenotypes: null, forceBaselinerChance: 0f, 
                developmentalStages: DevelopmentalStage.Adult);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            Pawn asker = slate.Get<Pawn>("asker");
            if (pawn.gender == Gender.Female)
            {
                var actions = new List<(float chance, Action action)>
                {
                    (0.5f, delegate
                    {
                        MakePregnant(pawn, asker);
                    }),
                    (0.3f, delegate
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            MakePregnant(pawn, asker);
                        }
                    }),
                    (0.1f, delegate
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            MakePregnant(pawn, asker);
                        }
                    })
                };
                actions.RandomElementByWeight(x => x.chance).action();
            }
            if (storeAs.GetValue(slate) != null)
            {
                QuestGen.slate.Set(storeAs.GetValue(slate), pawn);
            }
            QuestGen.AddToGeneratedPawns(pawn);
            if (!pawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(pawn);
            }
        }

        private void MakePregnant(Pawn pawn, Pawn asker)
        {
            Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.PregnantHuman, pawn);
            Rand.PushState(hediff_Pregnant.loadID);
            hediff_Pregnant.Severity = PregnancyUtility.GeneratedPawnPregnancyProgressRange.RandomInRange;
            hediff_Pregnant.TryGetComp<HediffComp_MessageAfterTicks>().ticksUntilMessage = -1;
            Pawn father = null;
            if (asker != null && asker.gender == Gender.Male && Rand.Chance(0.5f))
            {
                father = asker;
            }
            Rand.PopState();
            bool success;
            GeneSet inheritedGeneSet = PregnancyUtility.GetInheritedGeneSet(father, pawn, out success);
            if (success)
            {
                hediff_Pregnant.SetParents(null, father, inheritedGeneSet);
                pawn.health.AddHediff(hediff_Pregnant);
            }
        }

        public override bool TestRunInt(Slate slate)
        {
            if (GenDate.DaysPassed < 60)
            {
                return false;
            }
            return true;
        }
    }
}
