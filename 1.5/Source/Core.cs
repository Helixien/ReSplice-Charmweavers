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
        public static Color ThrallColor = new ColorInt(198, 122, 211).ToColor;

        static Core()
        {
            new Harmony("ReSpliceCharmweavers.Mod").PatchAll();
        }

        public static bool IsLovethrall(this Pawn pawn)
        {
            return IsLovethrall(pawn, out _);
        }

        public static int GetThrallAmount(this Pawn pawn)
        {
            return pawn.relations.DirectRelations.Where(x => x.def == RS_DefOf.RS_Thrall).Count();
        }

        public static bool IsLovethrall(this Pawn pawn, out Pawn master)
        {
            var hediff = pawn?.health?.hediffSet?.GetFirstHediffOfDef(RS_DefOf.RS_LoveThrall) as Hediff_LoveThrall;
            if (hediff != null)
            {
                master = hediff.master;
                return true;
            }
            else
            {
                master = null;
                return false;
            }
        }

        public static bool IsLovehexer(this Pawn pawn) => pawn.HasGene(RS_DefOf.RS_Lovehexer);

        public static bool HasGene(this Pawn pawn, GeneDef gene) 
        {
            return pawn?.genes?.HasGene(gene) ?? false;
        }

        public static T Clone<T>(this T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)inst?.Invoke(obj, null);
        }
    }
}
