using HarmonyLib;
using RimWorld;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(Verb_CastAbility), nameof(Verb_CastAbility.OrderForceTarget))]
public static class Verb_CastAbility_OrderForceTarget_Patch
{
    private static bool Prefix(Verb_CastAbility __instance, LocalTargetInfo target)
    {
        // If the ability is non-hostile, always let the method run normally. Otherwise, do the normal check.
        return !__instance.Ability.def.casterMustBeCapableOfViolence || Verb_OrderForceTarget_Patch.CanCastAbility(__instance, target, "RS.CannotUseHarmfulAbilitiesOnMaster");
    }
}