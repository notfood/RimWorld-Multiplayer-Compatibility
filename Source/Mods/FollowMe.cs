using HarmonyLib;
using Multiplayer.API;
using System;
using System.Collections;
using System.Collections.Generic;
using Verse;

namespace Multiplayer.Compat
{
    /// <summary>Misc. Robots by HaploX1</summary>
    /// <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=724602224"/>
    [MpCompatFor("Fluffy.FollowMe")]
    class FollowMe
    {
        public FollowMe(ModContentPack mod) => LongEventHandler.ExecuteWhenFinished(LatePatch);
        private static void LatePatch()
        {
            PatchingUtilities.SeedVerseRand("FollowMe.CinematicCamera:FollowNewSubject");
        }
    }
}
