using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch]
public static class Verb_OrderForceTarget_Patch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        // Only including abilities that are (or generally should be) harmful.
        // It's probably better to be safer here.
        var targetTypes = new[] { typeof(Verb), typeof(Verb_LaunchProjectileStatic), typeof(Verb_LaunchProjectileStaticOneUse), };

        // Verb_CastAbility and VFE's Verb_CastAbility have separate patches as they include an extra check if the ability is harmful.

        foreach (var type in targetTypes.Where(x => x != null))
        {
            var method = type.DeclaredMethod(nameof(ITargetingSource.OrderForceTarget));
            if (method != null)
                yield return method;
            else
                Log.ErrorOnce($"Trying to patch {nameof(ITargetingSource.OrderForceTarget)} for type {type.FullDescription()}, but it does not declare this method.", type.GetHashCode());
        }
    }

    private static bool Prefix(Verb __instance, LocalTargetInfo target) => CanCastAbility(__instance, target, "RS.CannotAttackMaster");

    public static bool CanCastAbility(Verb verb, LocalTargetInfo target, string failureTranslationKey)
    {
        // Make sure we're targeting a pawn
        if (target.Thing is not Pawn targetPawn)
            return true;
        // Make sure the caster is actually a pawn first
        if (!verb.CasterIsPawn)
            return true;
        // Check if the caster is a lovethrall
        if (!verb.CasterPawn.IsLovethrall(out var master))
            return true;
        // Check if the thrall's target is their master
        if (targetPawn != master)
            return true;

        Messages.Message(failureTranslationKey.Translate(verb.CasterPawn.Named("PAWN")), MessageTypeDefOf.RejectInput, false);
        return false;
    }
}