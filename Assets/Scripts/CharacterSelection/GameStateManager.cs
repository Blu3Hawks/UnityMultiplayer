using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        [SerializeField] private int bestOf = 7;

        [Networked] public int RoundIndex { get; set; }



        public override void Spawned()
        {
            if (Runner.IsServer)
            {
                characterSelectionManager.OnAllPlayersSelected += StartGame;
                RoundIndex = 0;
            }
        }

        public void StartGame()
        {
            RpcMatchStarted(bestOf);
            if (playerManagers != null && playerManagers.Count > 0)
            {
                foreach (PlayerManager player in playerManagers)
                {
                    RpcPlayerJoined(player.Id.GetHashCode(), player.CharacterName);
                    player.OnPlayerDeath += HandlePlayerDeath;
                }   
            }

            if (playerManagers != null) playersRemaining = playerManagers.Count;
            StartRound();
        }

        public void StartRound()
        {
            RoundIndex++;
            RpcRoundStarted(RoundIndex);
            if (playerManagers != null && playerManagers.Count > 0)
            {
                int i = 0;
                foreach (PlayerManager player in playerManagers)
                {
                    i++;
                    player.ToggleControls(true);
                    livingPlayers.Add(player);
                    player.TeleportToPos(characterSelectionManager.StartingPoints[i].transform.position);
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
            RpcPlayerDied(player.Id.GetHashCode());
            if (livingPlayers.Count == 1)
            {
                //Increase player score logic
                //UIRPC
                RpcRoundEnded(RoundIndex, livingPlayers[0].Id.GetHashCode());
                livingPlayers[0].Score += 1;
                RpcScoreSet(livingPlayers[0].Id.GetHashCode(), livingPlayers[0].Score);
                if (livingPlayers[0].Score >= bestOf)RpcMatchEnded(livingPlayers[0].Id.GetHashCode());
                livingPlayers.Clear();
                projectileSpawner.DespawnAll();
                projectileSpawner.StopSpawning();
                StartCoroutine(CountdownNextRound());
            }


        }
        private IEnumerator CountdownNextRound()
        {
            RpcRoundCountdown(RoundIndex, 3);
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

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcScoreSet(int actor, int score) {
            GameEvents.Raise(new GameEvents.ScoreSet(actor, score));
        }
        
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RpcPlayerJoined(int actor, string name) {
            GameEvents.Raise(actor, name);
        }

        #endregion
        
    }
}