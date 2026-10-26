using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(Dialog_FormCaravan), nameof(Dialog_FormCaravan.Notify_TransferablesChanged))]
public static class Dialog_FormCaravan_Notify_TransferablesChanged_Patch
{
    private static void Prefix(Dialog_FormCaravan __instance)
    {
        HashSet<Pawn> transferredMasters = [];
        List<(TransferableOneWay transferable, Pawn thrall, ThrallControlGroup controlGroup)> escortThralls = null;
        foreach (var transferable in __instance.transferables)
        {
            if (!transferable.IsThing)
                continue;

            for (var i = 0; i < transferable.things.Count; i++)
            {
                if (transferable.things[i] is not Pawn pawn)
                    continue;

                if (transferable.CountToTransferToDestination > 0 && pawn.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>() != null)
                    transferredMasters.Add(pawn);

                var controlGroup = pawn.GetThrallControlGroup();
                if (controlGroup != null && controlGroup.WorkMode == RS_DefOf.RS_Escort)
                    (escortThralls ??= []).Add((transferable, pawn, controlGroup));
            }
        }

        if (escortThralls is null)
        {
            return;
        }

        foreach (var (transferable, thrall, controlGroup) in escortThralls)
        {
            var master = controlGroup.Gene.pawn;
            var isMasterTransferred = transferredMasters.Contains(master);
            if (isMasterTransferred && transferable.CountToTransferToDestination <= 0)
            {
                Messages.Message("RS.MessageCaravanAddingEscortingThrall".Translate(thrall.Named("THRALL"), master.Named("MASTER")), thrall, MessageTypeDefOf.RejectInput, false);
                transferable.ForceToDestination(1);
            }
            else if (isMasterTransferred is false && transferable.CountToTransferToDestination > 0)
            {
                Messages.Message("RS.MessageCaravanRemovingEscortingThrall".Translate(thrall.Named("THRALL"), master.Named("MASTER")), thrall, MessageTypeDefOf.RejectInput, false);
                transferable.ForceToDestination(0);
            }
        }
    }
}
