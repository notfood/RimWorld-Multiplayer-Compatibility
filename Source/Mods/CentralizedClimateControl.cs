using System;
using Harmony;
using Multiplayer.API;
using Verse;

namespace Multiplayer.Compat
{
    /// <summary>Centralized Climate Control by Vasu Mahesh (coldtoad)</summary>
    /// <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=973091113"/>
    /// contribution to Multiplayer Compatibility by Cody Spring
    [MpCompatFor("Centralized Climate Control")]
    class CentralizedClimateControlCompat
    {

        static ISyncField _IsOperatingAtHighPower, _IsHeating, _IsBlocked, _IsStable, _IsBrokenDown, _IsPoweredOff, _IntakeTemperature, _TargetTemperature, _ConvertedTemperature, _DeltaTemperature;

        public CentralizedClimateControlCompat(ModContentPack mod)
        {
            Type type;
            {
                type = AccessTools.TypeByName("CentralizedClimateControl.CompAirFlowTempControl");

                _IsOperatingAtHighPower = MP.RegisterSyncField(type, "IsOperatingAtHighPower");
                _IsHeating = MP.RegisterSyncField(type, "IsHeating");
                _IsBlocked = MP.RegisterSyncField(type, "IsBlocked");
                _IsStable = MP.RegisterSyncField(type, "IsStable");
                _IsBrokenDown = MP.RegisterSyncField(type, "IsBrokenDown");
                _IsPoweredOff = MP.RegisterSyncField(type, "IsPoweredOff");
                _IntakeTemperature = MP.RegisterSyncField(type, "IntakeTemperature");
                _TargetTemperature = MP.RegisterSyncField(type, "TargetTemperature");
                _ConvertedTemperature = MP.RegisterSyncField(type, "ConvertedTemperature");
                _DeltaTemperature = MP.RegisterSyncField(type, "DeltaTemperature");
            }
            {
                {
                    var methods = new[] 
                    {
                        AccessTools.Method("CentralizedClimateControl.CompAirFlowTempControl:ResetFlowVariables"),
                        AccessTools.Method("CentralizedClimateControl.CompAirFlowTempControl:GenerateDelta"),
                        AccessTools.Method("CentralizedClimateControl.CompAirFlowTempControl:TickRare")
                    };
                    foreach (var method in methods)
                    {
                        MpCompat.harmony.Patch(method,
                            prefix: new HarmonyMethod(typeof(CentralizedClimateControlCompat), nameof(WatchPrefix)),
                            postfix: new HarmonyMethod(typeof(CentralizedClimateControlCompat), nameof(WatchPostfix))
                        );
                    }
                }
            }
        }

        static void WatchPrefix()
        {
            if (MP.IsInMultiplayer)
            {
                MP.WatchBegin();

                _IsOperatingAtHighPower.Watch();
                _IsHeating.Watch();
                _IsBlocked.Watch();
                _IsStable.Watch();
                _IsBrokenDown.Watch();
                _IsPoweredOff.Watch();
                _IntakeTemperature.Watch();
                _TargetTemperature.Watch();
                _ConvertedTemperature.Watch();
                _DeltaTemperature.Watch();

            }
        }

        static void WatchPostfix()
        {
            if (MP.IsInMultiplayer)
            {
                MP.WatchEnd();
            }
        }
    }
}