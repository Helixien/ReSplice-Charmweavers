using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(Dialog_FormCaravan), nameof(Dialog_FormCaravan.Notify_TransferablesChanged))]
public static class Dialog_FormCaravan_Notify_TransferablesChanged_Patch
{
    // Add/remove escorting pawns
    // Prefix rather than postfix, as the main method recalculates the travel supplies.
    // Adding or removing extra pawns will mess with the amount of travel supplies selected.
    private static void Prefix(Dialog_FormCaravan __instance)
    {
        foreach (var transferable in __instance.transferables)
        {
            if (!transferable.IsThing)
                continue;

            for (var i = 0; i < transferable.things.Count; i++)
            {
                var thing = transferable.things[i];
                if (thing is not Pawn pawn)
                    continue;

                var controlGroup = pawn.GetThrallControlGroup();
                if (controlGroup == null || controlGroup.WorkMode != RS_DefOf.RS_Escort)
                    continue;

                var isMasterTransferred = false;
                foreach (var otherTransferable in __instance.transferables)
                {
                    if (otherTransferable.IsThing && otherTransferable.things.Contains(controlGroup.Gene.pawn) && otherTransferable.CountToTransferToDestination > 0)
                    {
                        isMasterTransferred = true;
                        break;
                    }
                }

                if (isMasterTransferred && transferable.CountToTransferToDestination <= 0)
                {
                    Messages.Message("RS.MessageCaravanAddingEscortingThrall".Translate(pawn.Named("THRALL"), controlGroup.Gene.pawn.Named("MASTER")), pawn, MessageTypeDefOf.RejectInput, false);
                    transferable.ForceToDestination(1);
                }
                else if (!isMasterTransferred && transferable.CountToTransferToDestination > 0)
                {
                    Messages.Message("RS.MessageCaravanRemovingEscortingThrall".Translate(pawn.Named("THRALL"), controlGroup.Gene.pawn.Named("MASTER")), pawn, MessageTypeDefOf.RejectInput, false);
                    transferable.ForceToDestination(0);
                }
            }
        }
    }
}