using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

public class JobDriver_WorshipMaster : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

    public override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnNotCasualInterruptible(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_Interpersonal.WaitToBeAbleToInteract(pawn);
        yield return Toils_General.Do(() => pawn.interactions.TryInteractWith((Pawn)pawn.CurJob.targetA.Thing, RS_DefOf.RS_WorshipInteraction));
    }
}