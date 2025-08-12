using System.Collections.Generic;
using Fusion;
using Game_Events;
using UnityEngine;

namespace CharacterSelection
{
    public class GameStateManager : NetworkBehaviour
    {
        [SerializeField] private CharacterSelectionManager characterSelectionManager;
        private List<PlayerManager> players;
        
        private List<PlayerManager> playerManagers => characterSelectionManager.PlayerManagers;

        private int playersRemaining;

        public override void Spawned()
        {
            if(Runner.IsServer)
                characterSelectionManager.OnAllPlayersSelected += StartGame;
        }

        public void StartGame()
        {
            if (playerManagers != null && playerManagers.Count > 0)
            {
                foreach (PlayerManager player in playerManagers)
                {
                    player.OnPlayerDeath += HandlePlayerDeath;
                }    
            }
            StartRound();
        }

        public void StartRound()
        {
            if (playerManagers != null && playerManagers.Count > 0)
            {
                foreach (PlayerManager player in playerManagers)
                {
                    player.ToggleControls(true);
                }    
            }
            
            

            if (playerManagers != null) playersRemaining = playerManagers.Count;
        }

        private void HandlePlayerDeath(PlayerManager player)
        {
            player.ToggleControls(false);
            player.transform.position = new Vector3(100, 100, 100);//Teleport off map
        }

        #region Game Events

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcMatchStarted(int bestOf) {
            GameEvents.Raise(new GameEvents.MatchStart(bestOf));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcRoundCountdown(int roundIndex, float seconds) {
            GameEvents.Raise(new GameEvents.RoundCountdownStart(roundIndex, seconds));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcRoundStarted(int roundIndex) {
            GameEvents.Raise(new GameEvents.RoundStart(roundIndex));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcPlayerDied(int actorNumber) {
            GameEvents.Raise(new GameEvents.PlayerDied(actorNumber));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcRoundEnded(int roundIndex, int winnerActorNumber) {
            GameEvents.Raise(new GameEvents.RoundEnd(roundIndex, winnerActorNumber));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcMatchEnded(int winnerActorNumber) {
            GameEvents.Raise(new GameEvents.MatchEnd(winnerActorNumber));
        }

        #endregion
        
    }
}