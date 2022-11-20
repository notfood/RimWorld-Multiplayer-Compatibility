using Verse;

namespace Multiplayer.Compat
{
    /// <summary>Mountain Miner (Continued) by Mlie</summary>
    /// <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=2135339386"/>
    [MpCompatFor("mlie.mountainminer")]
    class MountainMinerContinued
    {
        public MountainMinerContinued(ModContentPack mod)
        {
            PatchingUtilities.PatchUnityRand("MountainMiner.Building_MountainDrill:Drill", false);
        }
    }
}
