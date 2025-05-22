using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class LobbyManager : LobbyManagerBase
{
    // Events
    public event UnityAction<List<SessionInfo>> onSessionListUpdated;
    public event UnityAction<PlayerRef, bool> onPlayersListChanged; 
    
    [Header("References")]
    [SerializeField] private NetworkRunner networkRunner;
    
    // Session list
    private List<SessionInfo> _sessionsList = new();

    public void StartSession()
    {
        networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "MyGameSession",
            OnGameStarted = OnGameStarted
        });
    }

    private void OnGameStarted(NetworkRunner obj)
    {
        Debug.Log("Game Started");
    }
    
    public async void JoinLobby(string input)
    {
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, input);

        if(result.Ok)
            Debug.Log("Lobby Joined Successfully");
    }

    public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        _sessionsList = sessionList;
        
        onSessionListUpdated?.Invoke(_sessionsList);
    }

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        onPlayersListChanged?.Invoke(player, true); // When player joined - invoke with true bool
    }

    public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        onPlayersListChanged?.Invoke(player, false); // When player left - invoke with false bool
    }
}
