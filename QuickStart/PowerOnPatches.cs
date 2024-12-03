using CG.Network;
using CG.Objects;
using CG.Ship.Hull;
using CG.Ship.Modules;
using HarmonyLib;
using Photon.Pun;
using System.Reflection;

namespace QuickStart
{
    internal class PowerOnPatches
    {
        internal static bool powerOn = false;

        [HarmonyPatch(typeof(HomunculusAndBiomassSocket))]
        class HomunculusAndBiomassSocketPatch
        {
            private static readonly MethodInfo SwitchToBiomassSocketMethod = AccessTools.Method(typeof(HomunculusAndBiomassSocket), "SwitchToBiomassSocket");
            private static readonly FieldInfo allowAutoDispenseField = AccessTools.Field(typeof(HomunculusAndBiomassSocket), "_allowAutoDispense");

            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            static void Awake(HomunculusAndBiomassSocket __instance)
            {
                if (!PhotonNetwork.IsMasterClient || !powerOn) return;
                SwitchToBiomassSocketMethod.Invoke(__instance, null);
            }

            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            static void Start(HomunculusAndBiomassSocket __instance)
            {
                if (!PhotonNetwork.IsMasterClient || !powerOn) return;
                allowAutoDispenseField.SetValue(__instance, false);
            }
        }

        [HarmonyPatch(typeof(CentralShipComputerModule))]
        class CentralShipComputerModulePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("Start")]
            static void Start(CentralShipComputerModule __instance)
            {
                if (!PhotonNetwork.IsMasterClient || !powerOn) return;
                CarryableObject homunculus = (CarryableObject)ObjectFactory.InstantiateSpaceObjectByGUID(new GUIDUnion("e1bdce573e8182b4d95aacb841301d7c"), __instance.transform.position, __instance.transform.rotation, null);
                __instance.CarryablesSockets[0].TryInsertCarryable(homunculus);
                __instance.ControlledPowerSystem.IsOn.ForceChange(true);
                powerOn = false;
            }
        }
    }
}
