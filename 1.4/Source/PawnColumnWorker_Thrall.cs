using RimWorld;
using UnityEngine;
using Verse;

namespace RareXenotypesSuccubus
{
    public class PawnColumnWorker_Thrall : PawnColumnWorker_Icon
    {
        public override Texture2D GetIconFor(Pawn pawn)
        {
            if (pawn.IsLoveThrall())
            {
                return Core.LoveThrallIcon;
            }
            return null;
        }

        public override string GetIconTip(Pawn pawn)
        {
            if (pawn.IsLoveThrall(out var master))
            {
                return "RX.ThrallOf".Translate(master.Named("PAWN"));
            }
            return null;
        }
    }
}
