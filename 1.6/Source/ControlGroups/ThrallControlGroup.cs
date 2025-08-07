using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReSpliceCharmweavers;

public class ThrallControlGroup : IExposable
{
    private List<AssignedThrall> assignedThralls = [];
    private Gene_PsychicEnthralling gene;
    private ThrallWorkModeDef workMode;
    private GlobalTargetInfo target = GlobalTargetInfo.Invalid;
    private List<Pawn> pawnsForReading;

    public List<Pawn> ThrallsForReading
    {
        get
        {
            pawnsForReading ??= [];
            pawnsForReading.Clear();

            foreach (var thrall in assignedThralls)
                pawnsForReading.Add(thrall.pawn);
            return pawnsForReading;
        }
    }

    public List<AssignedThrall> AssignedThralls => assignedThralls;

    public Gene_PsychicEnthralling Gene => gene;

    public ThrallWorkModeDef WorkMode => workMode;

    public GlobalTargetInfo Target => target;

    public int Index => Gene.controlGroups.IndexOf(this) + 1;

    public ThrallControlGroup(Gene_PsychicEnthralling gene)
    {
        this.gene = gene;
        workMode = RS_DefOf.RS_Work;
    }

    public bool TryUnassign(Pawn pawn) => assignedThralls.RemoveAll(x => x.pawn == pawn) > 0;

    public void Assign(Pawn pawn, bool showMessages = true)
    {
        foreach (var controlGroup in Gene.controlGroups)
            controlGroup.TryUnassign(pawn);

        assignedThralls.Add(new AssignedThrall(pawn));
        SetWorkModeForPawn(pawn, workMode, showMessages);
    }

    public void SetWorkMode(ThrallWorkModeDef workMode, bool showMessages = true) => SetWorkMode(workMode, GlobalTargetInfo.Invalid, showMessages);

    public void SetWorkMode(ThrallWorkModeDef workMode, GlobalTargetInfo target, bool showMessages = true)
    {
        if (this.workMode == workMode && this.target == target)
            return;

        this.workMode = workMode;
        this.target = target;

        foreach (var thrall in assignedThralls)
            SetWorkModeForPawn(thrall.pawn, workMode, showMessages);

        if (workMode is { followMasterOnCaravans: true })
        {
            var master = Gene.pawn;
            if (master.IsFormingCaravan())
            {
                var caravanLord = CaravanFormingUtility.GetFormAndSendCaravanLord(master);
                var pawns = Dialog_FormCaravan.AllSendablePawns(master.Map, false);
                for (var index = 0; index < assignedThralls.Count; index++)
                {
                    var thrall = assignedThralls[index].pawn;
                    if (pawns.Contains(thrall) && CaravanFormingUtility.GetFormAndSendCaravanLord(thrall) != caravanLord)
                    {
                        Messages.Message("RS.MessageCaravanAddingEscortingThrall".Translate(thrall.Named("THRALL"), master.Named("MASTER")), thrall, MessageTypeDefOf.RejectInput, false);
                        CaravanFormingUtility.LateJoinFormingCaravan(thrall, caravanLord);
                    }
                }
            }
        }
    }

    private void SetWorkModeForPawn(Pawn pawn, ThrallWorkModeDef workMode, bool showMessages)
    {
        pawn.jobs?.CheckForJobOverride();

        if (showMessages && workMode.warningIfNoWorkTags != WorkTags.None && !workMode.warningIfNoWorkTagsKey.NullOrEmpty() && pawn.WorkTagIsDisabled(workMode.warningIfNoWorkTags))
        {
            Messages.Message(workMode.warningIfNoWorkTagsKey.Translate(pawn.Named("THRALL"), workMode.Named("WORKMODE")), pawn, MessageTypeDefOf.NeutralEvent, false);
        }
    }

    public void ExposeData()
    {
        Scribe_Collections.Look(ref assignedThralls, nameof(assignedThralls), LookMode.Deep);
        Scribe_Defs.Look(ref workMode, nameof(workMode));
        Scribe_TargetInfo.Look(ref target, nameof(target));
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
            assignedThralls.RemoveAll(x => x.pawn == null || !x.pawn.IsLovethrall(out var master) || master != Gene.pawn);
    }
}