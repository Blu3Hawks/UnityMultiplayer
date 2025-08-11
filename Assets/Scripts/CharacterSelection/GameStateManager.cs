using System.Collections.Generic;
using Fusion;
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
        
        
        
    }
}