using CG.Client.UI;
using CG.GameLoopStateMachine.GameStates;
using Gameplay.Hub;
using Gameplay.Quests;
using HarmonyLib;
using Photon.Pun;
using ResourceAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    [HarmonyPatch]
    public class MiscPatches
    {
        [HarmonyPrefix, HarmonyPatch(typeof(FadeController), nameof(FadeController.FadeToBlack))]
        static bool FadeControllerFadeToBlack(Action callback = null, float duration = 1f)
        {
            callback?.Invoke();
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(FadeController), nameof(FadeController.FadeToClear))]
        static bool FadeControllerFadeToClear(Action callback = null, float duration = 1f)
        {
            callback?.Invoke();
            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MatchMakingMenu), nameof(MatchMakingMenu.OnTitleMenuEnter))]
        static void MatchMakingMenuOnTitleMenuEnter(MatchMakingMenu __instance)
        {
            __instance.HostPublicGame = false;
            SettingsManager.CurrentSettings.GeneralSettings.HostPublicGame.SetValue(0);
            __instance._playerLimit.SetValue(4);
            __instance._roomNameField.value = "Development Session (Unstable)";
            __instance.OnStartGameButtonPressed();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GSSpawn), nameof(GSSpawn.OnSceneUnloaded))]
        static void GSSpawnOnSceneUnloaded(Scene scene)
        {
            if(scene.buildIndex == CloneStarConstants.LoadingScreenSceneIndex && GameSessionManager.Instance.StartedSessionAsHost)
            {
                LaunchGame();
            }
        }

        static void LaunchGame()
        {
            if (!PhotonNetwork.IsMasterClient || !GameSessionManager.InHub) {
                return;
            }

            HubQuestManager questManager = HubQuestManager.Instance;
            
            PowerOnPatches.powerOn = true;


            if (HubShipManager.Instance.CurrentShipSelected == null)
            {
                ShipLoadoutDataDef ship = ResourceAssetContainer<ShipLoadoutDataContainer, ShipLoadoutData, ShipLoadoutDataDef>.Instance.AssetDescriptions[25]; // 11 = destroyer sentry, 25 = frigate sentry, 26 = frigate uber sentry
                HubShipManager.Instance.SelectShip(ship.AssetGuid.AsIntArray());
            }
            if (questManager.SelectedQuest == null) //Set to default values if no selected quest
                HubQuestManager.Instance.SelectQuest(DataTable<QuestTable>.Instance.EndlessQuest.AssetGuid);

            questManager.StartQuest(questManager.SelectedQuest, questManager.QuestStartType, questManager.questSeed, questManager.challengeSeed);
        }
    }
}
