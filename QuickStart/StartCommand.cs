using Gameplay.Quests;
using Photon.Pun;
using System.Collections.Generic;
using VoidManager.Chat.Router;

namespace QuickStart
{
    internal class StartCommand : ChatCommand
    {
        public override string[] CommandAliases()
        {
            return new string[] { "start" };
        }

        public override string Description()
        {
            return "Starts the game immediately";
        }

        public override List<Argument> Arguments()
        {
            return new List<Argument>() { new Argument(new string[] { "Power" }) };
        }

        public override string[] UsageExamples()
        {
            return new string[] { $"/{CommandAliases()[0]} [Power]\n" };
        }

        public override void Execute(string arguments)
        {
            if (!PhotonNetwork.IsMasterClient || !GameSessionManager.InHub) return;

            HubQuestManager questManager = HubQuestManager.Instance;
            PowerOnPatches.powerOn = false;

            string[] args = arguments.Split(' ');
            if (args.Length > 0 && args[0].ToLower() == "power")
            {
                PowerOnPatches.powerOn = true;
            }

            questManager.StartQuest(questManager.SelectedQuest ?? questManager.Quests[0]);
        }
    }
}
