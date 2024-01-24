using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers
{
    [HarmonyPatch(typeof(TaleRecorder), "RecordTale")]
    public static class TaleRecorder_RecordTale_Patch
    {
        public static void Postfix(Tale __result, TaleDef def, params object[] args)
    	{
    		if (__result != null && def == TaleDefOf.Breakup && args != null)
    		{
                var pawns = args.Cast<Pawn>().ToList();
                var initiator = pawns.FirstOrDefault();
                if (initiator?.Ideo != null)
                {
                    foreach (var precept in initiator.Ideo.PreceptsListForReading)
                    {
                        foreach (var comp in precept.def.comps)
                        {
                            if (comp is PreceptComp_Divorce preceptDivorce)
                            {
                                preceptDivorce.DoDivorceThought(initiator, pawns.Where(x => x != initiator).ToList(), precept);
                            }
                        }
                    }
                }
            }
    	}
    }
}
