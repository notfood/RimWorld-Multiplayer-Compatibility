using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using Harmony;
using Multiplayer.API;
using Verse;

namespace Multiplayer.Compat
{
    /// <summary>Quarry by Benjamin-S</summary>
    /// <see href="https://github.com/Benjamin-S/Quarry"/>
    /// contribution to Multiplayer Compatibility by Cody Spring
    [MpCompatFor("Quarry 1.0")]
    class QuarryCompat
    {
        static ISyncField _autoHaul, _mineModeToggle, _quarryPercent, _jobsCompleted, _firstSpawn, _facilityComp, _rockTypesUnder, _blocksUnder, _chunksUnder, _owners;
        public QuarryCompat(ModContentPack mod)
        {
            Type type;
            //fields
            {
                type = AccessTools.TypeByName("Quarry.Building_Quarry");

                _autoHaul = MP.RegisterSyncField(type, "autoHaul");
                _mineModeToggle = MP.RegisterSyncField(type, "mineModeToggle");
                _quarryPercent = MP.RegisterSyncField(type, "quarryPercent");
                _jobsCompleted = MP.RegisterSyncField(type, "jobsCompleted");
                _firstSpawn = MP.RegisterSyncField(type, "firstSpawn");
                _facilityComp = MP.RegisterSyncField(type, "facilityComp");
                _rockTypesUnder = MP.RegisterSyncField(type, "rockTypesUnder");
                _blocksUnder = MP.RegisterSyncField(type, "blocksUnder");
                _chunksUnder = MP.RegisterSyncField(type, "chunksUnder");
                _owners = MP.RegisterSyncField(type, "owners");
            }
            //gizmos
            MP.RegisterSyncDelegate(
                AccessTools.TypeByName("Quarry.Building_Quarry"),
                "<GetGizmos>d__59",
                "<>1__state"
            );

            MP.RegisterSyncMethod(AccessTools.Method("Quarry.JobDriver_MineQuarry:Collect"));
            MP.RegisterSyncMethod(AccessTools.Method("Quarry.JobDriver_MineQuarry:Mine"));

            {
                var methods = new[] { //Verse.Rand fixes
                    //AccessTools.Method("Quarry.OreDictionary:TakeOne"),
                    AccessTools.Method("Quarry.Building_Quarry:GiveResources"),
                    AccessTools.Method("Quarry.Building_Quarry:SpawnFilth"),
                    AccessTools.Method("Quarry.JobDriver_MineQuarry:MakeNewToils"),
                    AccessTools.Method("Quarry.JobDriver_MineQuarry:Collect"),
                    AccessTools.Method("Quarry.WorkGiver_MineQuarry:JobOnThing"),
                };
                foreach (var method in methods)
                {
                    MpCompat.harmony.Patch(method,
                        prefix: new HarmonyMethod(typeof(QuarryCompat), nameof(randomPrefix)),
                        postfix: new HarmonyMethod(typeof(QuarryCompat), nameof(randomPostfix))
                    );
                }
            }
            {
                var methods = new[] { //System.Random fixes
                    AccessTools.Method("Quarry.OreDictionary:TakeOne"),
                };
                foreach (var method in methods)
                {
                    MpCompat.harmony.Patch(method,
                        prefix: new HarmonyMethod(typeof(QuarryCompat), nameof(randomPrefix)),
                        postfix: new HarmonyMethod(typeof(QuarryCompat), nameof(randomPostfix)),
                        transpiler: new HarmonyMethod(typeof(QuarryCompat), nameof(RandFixerTranspiler))
                    );
                }
            }
            {
                var methods = new[]
                {
                        AccessTools.Method("Quarry.Building_Quarry:ResetFlowVariables"),
                        AccessTools.Method("Quarry.Building_Quarry:GenerateDelta"),
                        AccessTools.Method("Quarry.Building_Quarry:TickRare")
                    };
                foreach (var method in methods)
                {
                    MpCompat.harmony.Patch(method,
                        prefix: new HarmonyMethod(typeof(QuarryCompat), nameof(WatchPrefix)),
                        postfix: new HarmonyMethod(typeof(QuarryCompat), nameof(WatchPostfix))
                    );
                }
            }
        }


        static void WatchPrefix()
        {
            if (MP.IsInMultiplayer)
            {
                MP.WatchBegin();

                _autoHaul.Watch();
                _mineModeToggle.Watch();
                _quarryPercent.Watch();
                _jobsCompleted.Watch();
                _firstSpawn.Watch();
                _facilityComp.Watch();
                _rockTypesUnder.Watch();
                _blocksUnder.Watch();
                _chunksUnder.Watch();
                _owners.Watch();
            }
        }

        static void WatchPostfix()
        {
            if (MP.IsInMultiplayer)
            {
                MP.WatchEnd();
            }
        }

        static MethodInfo SystemNext = AccessTools.Method(type: typeof(Random), parameters: new Type[] { typeof(int), typeof(int) }, name: nameof(Random.Next));
        static MethodInfo RandRange = AccessTools.Method(type: typeof(Rand), parameters: new Type[] { typeof(int), typeof(int) }, name: nameof(Rand.Range));

        #region remove random
        static void randomPrefix()
        {
            if (MP.IsInMultiplayer)
            {
                Rand.PushState();
            }
        }
        static void randomPostfix()
        {
            if (MP.IsInMultiplayer)
            {
                Rand.PopState();
            }
        }

        public static IEnumerable<CodeInstruction> RandFixerTranspiler(IEnumerable<CodeInstruction> instr)
        {
            for (int i = 0; i < instr.Count(); i++)
            {
                CodeInstruction ci = instr.ElementAt(i);
                if (ci.opcode == OpCodes.Callvirt && ci.operand == SystemNext)
                {
                    CodeInstruction deleteldsfld = instr.ElementAt(i - 3);
                    deleteldsfld.opcode = OpCodes.Nop;
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: RandRange);
                }
                else yield return ci;
            }
        }//end
        #endregion
    }
}