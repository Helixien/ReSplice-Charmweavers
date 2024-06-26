﻿using RimWorld;
using Verse;

namespace ReSpliceCharmweavers
{
    public class ThoughtWorker_Precept_NotPregnant_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
        {
            return otherPawn.gender == Gender.Female
                && PregnancyUtility.GetPregnancyHediff(otherPawn) is null;
        }
    }
}
