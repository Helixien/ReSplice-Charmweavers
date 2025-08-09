using System;
using UnityEngine;
using Verse;

namespace ReSpliceCharmweavers;

public class ThrallWorkModeDef : Def
{
    [NoTranslate]
    public string iconPath;
    public Texture2D uiIcon;
    public int uiOrder;
    public Type workerClass = typeof(ThrallWorkModeDrawer);
    public bool followMasterOnCaravans;
    public bool allowWhileMasterAsleep = false;
    [NoTranslate]
    public string warningIfNoWorkTagsKey;
    public WorkTags warningIfNoWorkTags = WorkTags.None;
    [Unsaved]
    public ThrallWorkModeDrawer workerInt;

    public ThrallWorkModeDrawer Worker
    {
        get
        {
            if (workerInt == null)
            {
                workerInt = (ThrallWorkModeDrawer)Activator.CreateInstance(workerClass);
                workerInt.def = this;
            }

            return workerInt;
        }
    }

    public override void PostLoad()
    {
        if (!iconPath.NullOrEmpty())
            LongEventHandler.ExecuteWhenFinished(() => uiIcon = ContentFinder<Texture2D>.Get(iconPath));
    }
}