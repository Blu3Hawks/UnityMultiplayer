using System.Collections;
using System.Collections.Generic;
using Fusion;
using Game_Events;
using Projectiles;
using UnityEngine;

namespace CharacterSelection
{
    public class GameStateManager : NetworkBehaviour
    {
        [SerializeField] private CharacterSelectionManager characterSelectionManager;
        [SerializeField] private ProjectileSpawner projectileSpawner;
        private List<PlayerManager> players;
        private List<PlayerManager> livingPlayers = new List<PlayerManager>();
        
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

            if (playerManagers != null) playersRemaining = playerManagers.Count;
            StartRound();
        }

        public void StartRound()
        {
            if (playerManagers != null && playerManagers.Count > 0)
            {
                foreach (PlayerManager player in playerManagers)
                {
                    player.ToggleControls(true);
                    livingPlayers.Add(player);
                    player.TeleportToPos(Vector3.zero);
                }
                projectileSpawner.SpawnProjectiles();
                
            }
            

            if (playerManagers != null) playersRemaining = playerManagers.Count;
        }

        private void HandlePlayerDeath(PlayerManager player)
        {
            player.ToggleControls(false);
            player.TeleportToPos(new Vector3(100, 100, 100));//Teleport off map
            livingPlayers.Remove(player);
            if (livingPlayers.Count == 1)
            {
                //Increase player score logic
                //UIRPC
                livingPlayers[0].Score += 1;
                livingPlayers.Clear();
                projectileSpawner.DespawnAll();
                projectileSpawner.StopSpawning();
                StartCoroutine(CountdownNextRound());
            }


        }

        private IEnumerator CountdownNextRound()
        {
            RpcRoundCountdown(0, 3);
            yield return new WaitForSeconds(3f);
            StartRound();
        }

        #region Game Events

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcMatchStarted(int bestOf) {
            GameEvents.Raise(new GameEvents.MatchStart(bestOf));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcRoundCountdown(int roundIndex, float seconds) {
            GameEvents.Raise(new GameEvents.RoundCountdownStart(roundIndex, seconds));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcRoundStarted(int roundIndex) {
            GameEvents.Raise(new GameEvents.RoundStart(roundIndex));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcPlayerDied(int actorNumber) {
            GameEvents.Raise(new GameEvents.PlayerDied(actorNumber));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcRoundEnded(int roundIndex, int winnerActorNumber) {
            GameEvents.Raise(new GameEvents.RoundEnd(roundIndex, winnerActorNumber));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcMatchEnded(int winnerActorNumber) {
            GameEvents.Raise(new GameEvents.MatchEnd(winnerActorNumber));
        }

        #endregion
        
    }
}