using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace ReSpliceCharmweavers;

[HarmonyPatch(typeof(CaravanUIUtility), nameof(CaravanUIUtility.AddPawnsSections))]
public class CaravanUIUtility_AddPawnsSections_Patch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var addSectionCall = typeof(TransferableOneWayWidget).DeclaredMethod(nameof(TransferableOneWayWidget.AddSection));
        var extraSection = typeof(CaravanUIUtility_AddPawnsSections_Patch).DeclaredMethod(nameof(AddExtraSection));

        var whereCall = typeof(Enumerable) .GetDeclaredMethods()
            .FirstOrDefault(x => x.Name == nameof(Enumerable.Where) && x.GetParameters().Length == 2 && x.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2)
            .MakeGenericMethod(typeof(TransferableOneWay));
        var extraFilter = typeof(CaravanUIUtility_AddPawnsSections_Patch).DeclaredMethod(nameof(FilterThrallsFromPawns));

        var patchedOtherSections = false;
        var addedCustomSection = false;
        var isAfterSlavesSection = false;

        foreach (var instr in instructions)
        {
            yield return instr;

            // Remove love thralls from every other section
            if (!patchedOtherSections && instr.Calls(whereCall))
            {
                patchedOtherSections = true;
                yield return new CodeInstruction(OpCodes.Call, extraFilter);
            }
            // Insert our custom section
            else if (!addedCustomSection)
            {
                if (isAfterSlavesSection)
                {
                    if (instr.Calls(addSectionCall))
                    {
                        addedCustomSection = true;

                        // Load in the widget argument
                        yield return CodeInstruction.LoadArgument(0);
                        // Load in the transferables argument (don't use the local, as it already filters out love thralls)
                        yield return CodeInstruction.LoadArgument(1);
                        yield return new CodeInstruction(OpCodes.Call, extraSection);
                    }
                }
                else
                {
                    isAfterSlavesSection = instr.LoadsConstant("SlavesSection");
                }
            }
        }

        if (!patchedOtherSections || !addedCustomSection)
            throw new Exception($"Failed patching caravan UI to add a custom section for love thralls. Throwing an exception to prevent Harmony patch, as continuing could cause bugs. Patched other sections: {patchedOtherSections}. Added custom section: {addedCustomSection}.");
    }

    private static IEnumerable<TransferableOneWay> FilterThrallsFromPawns(IEnumerable<TransferableOneWay> transferables)
        => transferables.Where(x => !((Pawn)x.AnyThing).IsLovethrall());

    private static void AddExtraSection(TransferableOneWayWidget widget, IEnumerable<TransferableOneWay> transferables)
        => widget.AddSection("RS.LoveThralls".Translate(), transferables.Where(x => x.ThingDef.category == ThingCategory.Pawn && ((Pawn)x.AnyThing).IsLovethrall()));
}