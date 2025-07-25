using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(FloatMenuOptionProvider_CapturePawn), nameof(FloatMenuOptionProvider_CapturePawn.GetSingleOptionFor))]
public static class FloatMenuOptionProvider_CapturePawn_GetSingleOptionFor_Patch
{
    private static bool Prefix(Pawn clickedPawn, FloatMenuContext context, ref FloatMenuOption __result)
    {
        // Do nothing if ordered pawn is not a thrall
        if (!context.FirstSelectedPawn.IsLovethrall(out var master))
            return true;
        // Check if the thrall is targeting their master
        if (clickedPawn != master)
            return true;

        if (!clickedPawn.CanBeCaptured())
            return true;
        if (!HealthAIUtility.CanRescueNow(context.FirstSelectedPawn, clickedPawn, true))
            return true;

        // Replace the option with one that is inactive
        __result = new FloatMenuOption("CannotCapture".Translate() + ": " + "RS.CannotCaptureMaster".Translate(context.FirstSelectedPawn.Named("PAWN")), null);
        return false;
    }
}