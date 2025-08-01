using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReSpliceCharmweavers;

public static class AssignThrallToGroupUtility
{
    private static readonly HashSet<ThrallControlGroup> tmpControlGroups = [];
    private static List<ThrallControlGroup> tmpSelectedControlGroups = [];

    public static IEnumerable<ThrallControlGroup> SelectedControlGroups
    {
        get
        {
            tmpControlGroups.Clear();
            foreach (var pawn in Find.Selector.SelectedPawns)
            {
                if (pawn is { IsColonist: true } && pawn.IsLovethrall(out var master))
                {
                    var gene = master.genes?.GetFirstGeneOfType<Gene_PsychicEnthralling>();
                    if (gene != null)
                    {
                        foreach (var group in gene.controlGroups)
                        {
                            if (group != null && tmpControlGroups.Add(group))
                                yield return group;
                        }
                    }
                }
            }
        }
    }

    public static bool CanAssignPawn(Pawn p)
    {
        if (!p.IsColonist || !p.IsLovethrall(out var master))
            return false;

        foreach (var pawn in Find.Selector.SelectedPawns)
        {
            if (pawn.IsLovethrall(out var m) && master != m)
                return false;
        }

        return true;
    }

    private static bool CanAssignToControlGroup(Pawn pawn, ThrallControlGroup group)
        => pawn.IsLovethrall(out var master) && group.Gene.pawn == master;

    public static void CreateAssignThrallsFloatMenu()
    {
        tmpSelectedControlGroups.Clear();
        tmpSelectedControlGroups.AddRange(SelectedControlGroups);

        var options = new List<FloatMenuOption>();
        for (var i = 0; i < tmpSelectedControlGroups.Count; i++)
        {
            var selected = tmpSelectedControlGroups[i];

            if (Find.Selector.SelectedPawns.All(x => x.GetThrallControlGroup() == selected))
            {
                var floatMenuOption = new FloatMenuOption(
                    "RS.CannotAssignThrallToControlGroup".Translate(selected.Index) + " (" + selected.WorkMode.LabelCap.ToString() + ")" + ": " + "RS.AssignThrallAlreadyAssigned".Translate(),
                    null);
                options.Add(floatMenuOption);
            }
            else
            {
                var floatMenuOption2 = new FloatMenuOption(
                    "RS.AssignThrallToControlGroup".Translate(selected.Index) + " (" + selected.WorkMode.LabelCap.ToString() + ")",
                    () =>
                    {
                        foreach (var pawn in Find.Selector.SelectedPawns)
                        {
                            if (CanAssignToControlGroup(pawn, selected))
                                selected.Assign(pawn);
                        }
                    });
                options.Add(floatMenuOption2);
            }
        }

        Find.WindowStack.Add(new FloatMenu(options));

        tmpSelectedControlGroups.Clear();
    }
}