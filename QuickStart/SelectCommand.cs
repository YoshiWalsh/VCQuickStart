
using Gameplay.Hub;
using Photon.Pun;
using ResourceAssets;
using VoidManager.Chat.Router;
using VoidManager.Utilities;

namespace QuickStart
{
    internal class SelectCommand : ChatCommand
    {
        public override string[] CommandAliases()
        {
            return new string[] { "select" };
        }

        public override string Description()
        {
            return "Select a ship to use";
        }

        public override string[] UsageExamples()
        {
            return new string[] { $"/{CommandAliases()[0]} <ship loadout index>" };
        }

        public override void Execute(string arguments)
        {
            if (!PhotonNetwork.IsMasterClient || !GameSessionManager.InHub) return;

            string[] args = arguments.Split(' ');
            if (args.Length > 0 && int.TryParse(args[0], out int index))
            {
                ShipLoadoutDataDef ship = ResourceAssetContainer<ShipLoadoutDataContainer, ShipLoadoutData, ShipLoadoutDataDef>.Instance.AssetDescriptions[index];
                HubShipManager.Instance.SelectShip(ship.AssetGuid.AsIntArray());
                Messaging.Notification($"Selected {ship.ShipContextInfo.HeaderText}: {ship}", default);
            }
        }
    }
}
