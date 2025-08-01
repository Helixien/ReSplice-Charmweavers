using JetBrains.Annotations;
using Verse;

namespace ReSpliceCharmweavers;

public class AssignedThrall : IExposable
{
    public Pawn pawn;
    public int tickAssigned;

    [UsedImplicitly]
    public AssignedThrall()
    {
    }

    public AssignedThrall(Pawn pawn)
    {
        this.pawn = pawn;
        tickAssigned = Find.TickManager.TicksGame;
    }

    public void ExposeData()
    {
        Scribe_References.Look(ref pawn, nameof(pawn));
        Scribe_Values.Look(ref tickAssigned, nameof(tickAssigned));
    }
}