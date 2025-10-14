using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
{
    [StaticConstructorOnStartup]
    public static class Core
    {
        public static bool VRESangouphageActive = ModsConfig.IsActive("vanillaracesexpanded.sanguophage");
        public static Texture2D LoveThrallIcon = ContentFinder<Texture2D>.Get("UI/Icons/Thrall");
        public static Texture2D SelectAllThrallsIcon = ContentFinder<Texture2D>.Get("UI/Icons/Gizmos/SelectAllThralls");
        public static Texture2D SelectMasterIcon = ContentFinder<Texture2D>.Get("UI/Icons/Gizmos/SelectMaster");
        public static Color ThrallColor = new ColorInt(198, 122, 211).ToColor;

        static Core()
        {
            var harmony = new Harmony("ReSpliceCharmweavers.Mod");
            harmony.PatchAllUncategorized();

            if (ModsConfig.IsActive("rim.job.world"))
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
            return pawn.relations.DirectRelations.Count(x => x.def == RS_DefOf.RS_Thrall);
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
    }
}
