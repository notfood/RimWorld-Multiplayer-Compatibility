﻿using System.Reflection;
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace Multiplayer.Compat
{
    /// <summary>Common Sense by avil</summary>
    /// <see href="https://github.com/catgirlfighter/RimWorld_CommonSense"/>
    /// <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=1561769193"/>
    [MpCompatFor("avilmask.CommonSense")]
    class CommonSense
    {
        private static ISyncField shouldUnloadSyncField;
        private static MethodInfo getCompUnlockerCheckerMethod;

        public CommonSense(ModContentPack mod)
        {
            var type = AccessTools.TypeByName("CommonSense.CompUnloadChecker");
            // We need to make a sync worker for this Comp, as it is initialized dynamically and might not exists at the time
            MP.RegisterSyncWorker<ThingComp>(SyncComp, type);
            shouldUnloadSyncField = MP.RegisterSyncField(AccessTools.Field(type, "ShouldUnload"));
            // The GetChecker method either gets an existing, or creates a new comp
            getCompUnlockerCheckerMethod = AccessTools.Method(type, "GetChecker");

            // Watch unload bool changes
            MpCompat.harmony.Patch(AccessTools.Method("CommonSense.Utility:DrawThingRow"),
                prefix: new HarmonyMethod(typeof(CommonSense), nameof(CommonSensePatchPrefix)),
                postfix: new HarmonyMethod(typeof(CommonSense), nameof(CommonSensePatchPostix)));

            // RNG Patch
            PatchingUtilities.PatchUnityRand("CommonSense.JobGiver_Wander_TryGiveJob_CommonSensePatch:Postfix", false);
        }

        private static void CommonSensePatchPrefix(Thing thing)
        {
            if (MP.IsInMultiplayer)
            {
                MP.WatchBegin();
                var comp = getCompUnlockerCheckerMethod.Invoke(null, new object[] { thing, false, false });
                shouldUnloadSyncField.Watch(comp);
            }
        }

        private static void CommonSensePatchPostix()
        {
            if (MP.IsInMultiplayer)
                MP.WatchEnd();
        }

        private static void SyncComp(SyncWorker sync, ref ThingComp thing)
        {
            if (sync.isWriting)
                sync.Write<Thing>(thing.parent);
            else
                // Get existing or create a new comp
                thing = (ThingComp)getCompUnlockerCheckerMethod.Invoke(null, new object[] { sync.Read<Thing>(), false, false });
        }
    }
}
