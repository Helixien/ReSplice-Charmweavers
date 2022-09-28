using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RareXenotypesSuccubus
{
    public static class OxytocinUtility
    {
        public static void OffsetOxytocin(Pawn pawn, float offset, bool applyStatFactor = true)
        {
            Gene_Oxytocin gene_Oxytocin = pawn.genes?.GetFirstGeneOfType<Gene_Oxytocin>();
            if (gene_Oxytocin != null)
            {
                gene_Oxytocin.Value += offset;
            }
        }
        public static void TickResourceDrain(IGeneResourceDrain drain)
        {
            if (drain.CanOffset && drain.Resource != null)
            {
                OffsetResource(drain, (0f - drain.ResourceLossPerDay) / 60000f);
            }
        }


        public static void OffsetResource(IGeneResourceDrain drain, float amnt)
        {
            drain.Resource.Value += amnt;
        }

        public static IEnumerable<Gizmo> GetResourceDrainGizmos(IGeneResourceDrain drain)
        {
            if (DebugSettings.ShowDevGizmos && drain.Resource != null)
            {
                Gene_Resource resource = drain.Resource;
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "DEV: " + resource.ResourceLabel + " -10";
                command_Action.action = delegate
                {
                    OffsetResource(drain, -0.1f);
                };
                yield return command_Action;
                Command_Action command_Action2 = new Command_Action();
                command_Action2.defaultLabel = "DEV: " + resource.ResourceLabel + " +10";
                command_Action2.action = delegate
                {
                    OffsetResource(drain, 0.1f);
                };
                yield return command_Action2;
            }
        }
    }
}
