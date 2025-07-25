using HarmonyLib;
using VEF.Abilities;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(Verb_CastAbility), nameof(Verb_CastAbility.OrderForceTarget))]
public static class Verb_CastAbility_VEF_OrderForceTarget_Patch
{
    private static bool Prefix(Verb_CastAbility __instance, LocalTargetInfo target)
    {
        // If the ability is either positive or unspecified, always let the method run normally. Otherwise, do the normal check.
        return __instance.ability.def.isPositive != false || Verb_OrderForceTarget_Patch.CanCastAbility(__instance, target, "RS.CannotUseHarmfulAbilitiesOnMaster");
    }
}