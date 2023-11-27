﻿using RimWorld;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers
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
                return "RS.ThrallOf".Translate(master.Named("PAWN"));
            }
            return null;
        }
    }
}
