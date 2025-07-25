using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch]
public static class FloatMenuUtility_DisableAttack_Patch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        // Methods that either use the built-in pawn's attack (their melee attack)
        // along with group attacks gizmos using mixed weapon types.
        yield return typeof(FloatMenuUtility).DeclaredMethod(nameof(FloatMenuUtility.GetMeleeAttackAction));
        yield return typeof(FloatMenuUtility).DeclaredMethod(nameof(FloatMenuUtility.GetRangedAttackAction));
    }

    private static bool Prefix(Pawn pawn, LocalTargetInfo target, ref string failStr)
    {
        // Check if targeting a pawn
        if (target.Thing is not Pawn targetPawn)
            return true;
        // Check if the pawn is a lovethrall
        if (!pawn.IsLovethrall(out var master))
            return true;
        // Check if the thrall is targetting their master
        if (targetPawn != master)
            return true;

        failStr = "RS.CannotAttackMaster".Translate(pawn.Named("PAWN")).CapitalizeFirst();
        return false;
    }
}