using Harmony;
using Multiplayer.API;
using Verse;

namespace Multiplayer.Compat
{
    /// <summary>Mortar Accuracy by Hob Took</summary>
    /// <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=1729446857"/>
    /// contribution to Multiplayer Compatibility by Cody Spring
    [MpCompatFor("Mortar Accuracy")]
    class MortarAccuracyCompat
    {
        public MortarAccuracyCompat(ModContentPack mod)
        {
            //RNG Fix
            {
                var methods = new[] {
                    AccessTools.Method("MortarAccuracy.Verb_LaunchProjectile_MortarMod:FireProjectile"),
                };

                foreach (var method in methods)
                {
                    MpCompat.harmony.Patch(method,
                        prefix: new HarmonyMethod(typeof(MortarAccuracyCompat), nameof(FixRNGPre)),
                        postfix: new HarmonyMethod(typeof(MortarAccuracyCompat), nameof(FixRNGPos))
                    );
                }
            }
        }
        static void FixRNGPre() => Rand.PushState();
        static void FixRNGPos() => Rand.PopState();
    }
}