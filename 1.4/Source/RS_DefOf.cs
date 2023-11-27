using RimWorld;
using Verse;
using VFECore;

namespace ReSpliceCharmweavers
{
    [DefOf]
    public static class RS_DefOf
    {
        public static GeneDef RS_Matrianic;
        public static PawnRelationDef RS_Master;
        public static PawnRelationDef RS_Thrall;
        public static HediffDef RS_LoveThrall;
        public static ThoughtDef RS_BecameThrallMood;
        public static ThoughtDef RS_GotSomeLovinThrall;
        public static InteractionDef Breakup;
        public static GeneDef RS_Beauty_Angelic;
        public static GeneDef RS_Libido_None;
        public static GeneDef RS_LoveFeed;
        public static GeneDef RS_PsychicEnthralling;
        [MayRequire("vanillaracesexpanded.sanguophage")] public static GeneDef VRE_SanguoFeeder;
        public static GeneDef RS_MultiPregnancy;
        public static GeneDef RS_TemperatureInsensitive;
        public static ThoughtDef RS_BrokenEnthrallment, RS_EnthralledPrisoner, RS_EnthralledMe;
        public static PregnancyApproachDef RS_LovinForHemogen;
        public static XenotypeDef RS_Charmweaver;
    }
}
