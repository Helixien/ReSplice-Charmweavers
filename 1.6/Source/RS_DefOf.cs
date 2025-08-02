using RimWorld;
using VEF.Pawns;
using Verse;

namespace ReSpliceCharmweavers
{
    [DefOf]
    public static class RS_DefOf
    {
        public static GeneDef RS_Matrianic;
        public static PawnRelationDef RS_Master;
        public static PawnRelationDef RS_Thrall;
        public static HediffDef RS_LoveThrall;
        public static HediffDef RS_RecentImpregnation;
        public static HediffDef RS_RecentPregnancy;
        public static ThoughtDef RS_BecameThrallMood;
        public static ThoughtDef RS_GotSomeLovinThrall;
        public static InteractionDef Breakup;
        public static GeneDef RS_Beauty_Angelic;
        public static GeneDef RS_Libido_None;
        public static GeneDef RS_LoveFeed;
        public static GeneDef RS_Lovehexer;
        [MayRequire("vanillaracesexpanded.sanguophage")] public static GeneDef VRE_SanguoFeeder;
        public static GeneDef RS_MultiPregnancy;
        public static GeneDef RS_TemperatureInsensitive;
        public static GeneDef RS_LovethrallStraight;
        public static GeneDef RS_LovethrallGay;
        public static GeneDef RS_LovethrallBisexual;
        public static ThoughtDef RS_BrokenEnthrallment, RS_EnthralledPrisoner, RS_EnthralledMe;
        public static PregnancyApproachDef RS_LovinForHemogen;
        public static XenotypeDef RS_Charmweaver;
        [MayRequireIdeology] public static PreceptDef RS_Divorce_Forbidden, RS_Divorce_MaleOnly, RS_Divorce_FemaleOnly,
            RS_Marriage_MaleOnly, RS_Marriage_FemaleOnly, RS_Marriage_Abhorrent;
        [MayRequireIdeology] public static HistoryEventDef RS_PropagateLovehexerGene;
        public static ThingDef RS_EnthralledMote;
        public static EffecterDef Bloodfeed_Warmup;
        public static ThrallWorkModeDef RS_Work;
        public static ThrallWorkModeDef RS_Escort;
        public static JobDef RS_Worship;
        public static InteractionDef RS_WorshipInteraction;
    }
}
