using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    [DefOf]
    public static class RS_DefOf
    {
        public static GeneDef RS_Matrianic;
        public static PawnRelationDef RX_Master;
        public static PawnRelationDef RX_Thrall;
        public static HediffDef RX_LoveThrall;
        public static ThoughtDef RX_BecameThrallMood;
        public static ThoughtDef RX_GotSomeLovinThrall;
        public static InteractionDef Breakup;
        public static GeneDef RS_Beauty_Angelic;
        public static GeneDef RS_Libido_None;
        public static GeneDef RS_LoveFeed;
        public static GeneDef RS_PsychicEnthralling;
        [MayRequire("vanillaracesexpanded.sanguophage")] public static GeneDef VRE_SanguoFeeder;
        public static GeneDef RS_MultiPregnancy;
        public static GeneDef RS_TemperatureInsensitive;
    }
}
