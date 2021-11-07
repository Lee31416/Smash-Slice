using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.NetworkRoom
{
    public class NetworkRoomManagerExt : NetworkRoomManager
    {
        public Dictionary<int, PlayerControl> players { get; } = new Dictionary<int, PlayerControl>();

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            players.Add(roomPlayer.GetComponent<NetworkRoomPlayer>().index, gamePlayer.GetComponent<PlayerControl>());
            return true;
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            base.OnRoomServerSceneChanged(sceneName);
            if (sceneName == "Assets/Scenes/RoomScene.unity")
            {
                players.Clear();
            }
        }

        public void UpdateStatus(TextMeshProUGUI textObj)
        {
            switch (NetworkServer.active)
            {
                case true when NetworkClient.active:
                    textObj.text = ($"<b>Host</b>: running via {Transport.activeTransport}");
                    break;
                case true:
                    textObj.text = ($"<b>Server</b>: running via {Transport.activeTransport}");
                    break;
                default:
                {
                    if (NetworkClient.isConnected)
                    {
                        textObj.text = ($"<b>Client</b>: connected to {networkAddress} via {Transport.activeTransport}");
                    }

                    break;
                }
            }
        }
        
        public void HandleStopButtons()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                StopClient();
            }
            else if (NetworkServer.active)
            {
                StopServer();
            }
        }

        /*
            This code below is to demonstrate how to do a Start button that only appears for the Host player
            showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
            all players are ready, but if a player cancels their ready state there's no callback to set it back to false
            Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
            Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
            is set as DontDestroyOnLoad = true.
        */

        bool showStartButton;

        public override void OnRoomServerPlayersReady()
        {
            // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            showStartButton = true;
#endif
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                // set to false to hide it in the game scene
                showStartButton = false;

                ServerChangeScene(GameplayScene);
            }
        }
    }
}
