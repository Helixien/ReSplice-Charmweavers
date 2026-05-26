using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ReSpliceCharmweavers
{
    [StaticConstructorOnStartup]
    public static class Core
    {
        public static bool VRESangouphageActive = ModLister.AnyModActiveNoSuffix(["vanillaracesexpanded.sanguophage"]);
        public static Texture2D LoveThrallIcon = ContentFinder<Texture2D>.Get("UI/Icons/Thrall");
        public static Texture2D SelectAllThrallsIcon = ContentFinder<Texture2D>.Get("UI/Icons/Gizmos/SelectAllThralls");
        public static Texture2D SelectMasterIcon = ContentFinder<Texture2D>.Get("UI/Icons/Gizmos/SelectMaster");
        public static Color ThrallColor = new ColorInt(198, 122, 211).ToColor;
        public const string LatePatchesCategory = "LatePatches";

        static Core()
        {
            var harmony = new Harmony("ReSpliceCharmweavers.Mod");
            harmony.PatchAllUncategorized();
            // Patches that rely on stuff being more initialized
            LongEventHandler.ExecuteWhenFinished(() => harmony.PatchCategory(LatePatchesCategory));

            if (ModLister.AnyModActiveNoSuffix(["rim.job.world"]))
            {
                // Those 2 patches specifically are relying on one another. Make sure that either both or none of them are applied, to avoid issues.
                var before = CountOurPatches();
                harmony.PatchCategory(RJW_VFEPregnancyApproach_GetPreference_Patch.RJWTryForBaby_VFEPregnancyApproach_Patches);
                if (before + 2 != CountOurPatches())
                    harmony.UnpatchCategory(RJW_VFEPregnancyApproach_GetPreference_Patch.RJWTryForBaby_VFEPregnancyApproach_Patches);
            }

            int CountOurPatches() => Harmony.GetAllPatchedMethods().Select(Harmony.GetPatchInfo).Count(x => x.Owners.Contains(harmony.Id));
        }

        public static bool IsLovethrall(this Pawn pawn)
        {
            return IsLovethrall(pawn, out _);
        }

        public static int GetThrallAmount(this Pawn pawn)
        {
            return pawn.relations.DirectRelations.Count(x => x.def == RS_DefOf.RS_Thrall && x.otherPawn is { Dead: false });
        }

        public static Pawn GetMaster(this Pawn pawn)
        {
            return (pawn?.health?.hediffSet?.GetFirstHediffOfDef(RS_DefOf.RS_LoveThrall) as Hediff_LoveThrall)?.master;
        }

        public static bool IsLovethrall(this Pawn pawn, out Pawn master)
        {
            master = pawn.GetMaster();
            return master != null;
        }

        public static bool IsLovehexer(this Pawn pawn) => pawn.HasActiveGene(RS_DefOf.RS_Lovehexer);

        public static bool HasActiveGene(this Pawn pawn, GeneDef gene)
        {
            return pawn?.genes?.HasActiveGene(gene) ?? false;
        }

        public static T Clone<T>(this T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)inst?.Invoke(obj, null);
        }

        public static ThrallControlGroup GetThrallControlGroup(this Pawn pawn)
        {
            if (!pawn.IsLovethrall(out var master))
                return null;
            return master.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>()?.GetControlGroup(pawn);
        }

        public static Pawn FindPawnToEnthrall(Pawn lovehexer)
        {
            if (!lovehexer.Spawned)
                return null;

            var ability = lovehexer.abilities.GetAbility(RS_DefOf.RS_PsychicEnthralling);
            if (ability == null || !ability.CanCast)
                return null;

            var colonistsWithRoles = SimplePool<List<Pawn>>.Get();
            var colonists = SimplePool<List<Pawn>>.Get();
            var prisoners = SimplePool<List<Pawn>>.Get();

            try
            {
                var pawns = lovehexer.Map.mapPawns.AllPawnsSpawned;
                for (var i = 0; i < pawns.Count; i++)
                {
                    var pawn = pawns[i];
                    // Find a different pawn of same faction, a prisoner, or a slave
                    if (pawn != lovehexer && pawn.RaceProps.Humanlike && !pawn.IsLovehexer() && !pawn.IsLovethrall() && !pawn.IsBurning() && !pawn.InAggroMentalState && CompAbilityEffect_Menticide.CanEverBeTargeted(pawn, false, ability) && lovehexer.CanReserveAndReach(pawn, PathEndMode.Touch, Danger.Deadly))
                    {
                        if (pawn.IsPrisoner && pawn.guest.HostFaction == lovehexer.Faction || pawn.IsSlave && pawn.Faction == lovehexer.Faction)
                            prisoners.Add(pawn);
                        else if (pawn.Faction == lovehexer.Faction)
                        {
                            if (pawn.Ideo?.GetRole(pawn) != null)
                                colonistsWithRoles.Add(pawn);
                            else
                                colonists.Add(pawn);
                        }
                    }
                }

                return colonistsWithRoles.RandomElementWithFallback() ?? colonists.RandomElementWithFallback() ?? prisoners.RandomElementWithFallback();
            }
            finally
            {
                colonistsWithRoles.Clear();
                colonists.Clear();
                prisoners.Clear();

                SimplePool<List<Pawn>>.Return(colonistsWithRoles);
                SimplePool<List<Pawn>>.Return(colonists);
                SimplePool<List<Pawn>>.Return(prisoners);
            }
        }
    }
}
