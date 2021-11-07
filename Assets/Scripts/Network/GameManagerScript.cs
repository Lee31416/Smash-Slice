using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.NetworkRoom;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class GameManagerScript: NetworkBehaviour
    {
        [SerializeField] private GameObject _waitingStartUi;
        [SerializeField] private GameObject _winUi;
        [SerializeField] private TextMeshProUGUI _winnerNameText;
 
        private NetworkRoomManagerExt _manager;
        private Dictionary<int, PlayerControl> _alivePlayers = new Dictionary<int, PlayerControl>();

        private void Start()
        {
            if (!isServer) return;

            _manager = NetworkManager.singleton.GetComponentInChildren<NetworkRoomManagerExt>();

            StartCoroutine(GameLoop());
        }

        private IEnumerator GameLoop()
        {
            DisableAllPlayers();
            
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());
        }

        private IEnumerator RoundStarting()
        {
            yield return new WaitForSeconds(7);
            
            RpcDeactivateWaitingUi();
            EnableAllPlayers();
        }

        [ClientRpc]
        private void RpcDeactivateWaitingUi()
        {
            _waitingStartUi.SetActive(false);

        }

        private IEnumerator RoundPlaying()
        {
            //TODO Update timer
            
            while (!IsOnePlayerLeft())
            { 
                yield return null;
            }
        }
        
        private IEnumerator RoundEnding()
        {
            yield return new WaitForSeconds(2);
            
            DisableAllPlayers();
            
            RpcShowWinUi(GetGameWinner());
        }

        [ClientRpc]
        private void RpcShowWinUi(PlayerControl winner)
        {
            _winnerNameText.text = winner == null ? "Tie" : winner.name;
            _winUi.SetActive(true);
        }
        
        private void EnableAllPlayers()
        {
            foreach (var player in _manager.players)
            {
                player.Value.RpcUnFreezeAllClient();
            }
        }
        
        private void DisableAllPlayers()
        {
            foreach (var player in _manager.players)
            {
                player.Value.RpcFreezeAllClient();
            }
        }
        
        private PlayerControl GetGameWinner()
        {
            return _manager.players.Where(player => player.Value.isAlive).Select(player => player.Value).FirstOrDefault();
        }
        
        private bool IsOnePlayerLeft()
        {
            var alivePlayerCount = _manager.players.Count;
            var temp = _manager.players.ToList();
            foreach (var player in temp)
            {
                if (player.Value.isAlive) continue;
                alivePlayerCount--;
                _alivePlayers.Remove(player.Key);
            }
            return alivePlayerCount <= 1;
        }
    }
}