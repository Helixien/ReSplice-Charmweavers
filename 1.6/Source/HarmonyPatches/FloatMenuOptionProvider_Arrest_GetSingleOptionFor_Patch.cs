using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(FloatMenuOptionProvider_Arrest), nameof(FloatMenuOptionProvider_Arrest.GetSingleOptionFor))]
public static class FloatMenuOptionProvider_Arrest_GetSingleOptionFor_Patch
{
    private static bool Prefix(Pawn clickedPawn, FloatMenuContext context, ref FloatMenuOption __result)
    {
        // Do nothing if ordered pawn is not a thrall
        if (!context.FirstSelectedPawn.IsLovethrall(out var master))
            return true;
        // Check if the thrall is targeting their master
        if (clickedPawn != master)
            return true;

        // Verify that the float menu should actually be shown in the first place
        if (!clickedPawn.CanBeArrestedBy(context.FirstSelectedPawn))
            return true;
        if (clickedPawn.Downed && clickedPawn.guilt.IsGuilty)
            return true;
        if (!context.FirstSelectedPawn.Drafted && (!clickedPawn.IsWildMan() || clickedPawn.IsPrisonerOfColony))
            return true;

        // Replace the option with one that is inactive
        __result = new FloatMenuOption("CannotArrest".Translate() + ": " + "RS.CannotArrestMaster".Translate(context.FirstSelectedPawn.Named("PAWN")), null);
        return false;
    }
}