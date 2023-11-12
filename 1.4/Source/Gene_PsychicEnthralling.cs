using System.Collections.Generic;
using Verse;

namespace ReSpliceCharmweavers
{
    public class Gene_PsychicEnthralling : Gene
    {
        public Gizmo_ThrallAmount gizmo;
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }
            gizmo ??= new Gizmo_ThrallAmount
            {
                pawn = this.pawn
            };
            yield return gizmo;
        }
    }
}
