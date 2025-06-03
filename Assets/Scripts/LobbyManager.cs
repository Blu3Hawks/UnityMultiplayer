using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LobbyManager : LobbyManagerBase
{
    // Events
    public event UnityAction<List<SessionInfo>> onSessionListUpdated;
    public event UnityAction<PlayerRef, bool> onPlayersListChanged;

    [Header("References")]
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private GameObject readyManagerGeneric;
    [SerializeField] private TextMeshProUGUI lobbyName;



    public ReadyManager readyManagerInstance;

    // Session list
    private List<SessionInfo> _sessionsList = new();

    //private variables
    private int amountOfPlayers;
    //properties
    public int AmountOfPlayers { get { return  networkRunner.SessionInfo.PlayerCount; } }

    //scene const names
    public const string GAME_SCENE_NAME = "MyLobby";

    //static reference
    public static LobbyManager Instance { get; private set; }

    public async void StartSession()
    {
        Debug.Log(lobbyName.text);
        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = lobbyName.text,
            OnGameStarted = OnGameStarted,
        });

        onSessionListUpdated?.Invoke(_sessionsList);
    }
    void Awake()
    {
        networkRunner.AddCallbacks(this);
    }

    public void StartMatch()
    {
        if (networkRunner.IsSceneAuthority)
            networkRunner.LoadScene(GAME_SCENE_NAME);
    }

    private void OnGameStarted(NetworkRunner obj)
    {
        Debug.Log("Game Started + Chen HaHomo");
        if (networkRunner.IsSharedModeMasterClient)
            networkRunner.Spawn(readyManagerGeneric);
        amountOfPlayers++;

        onSessionListUpdated?.Invoke(_sessionsList);
        Debug.Log(amountOfPlayers);

    }

    public async void JoinLobby()
    {
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, networkRunner.SessionInfo.Name);
        Debug.Log(lobbyName.text);

        if (result.Ok)
        {
            Debug.Log("Lobby Joined Successfully");
            
        }
    }

    public override void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        _sessionsList = sessionList;
        onSessionListUpdated?.Invoke(_sessionsList);
    }

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        amountOfPlayers = runner.SessionInfo.PlayerCount;
        onPlayersListChanged?.Invoke(player, true); // When player joined - invoke with true bool
        Debug.Log($"playercount: {runner.SessionInfo?.PlayerCount}");
    }

    public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        amountOfPlayers--;
        onPlayersListChanged?.Invoke(player, false); // When player left - invoke with false bool
        Debug.Log(amountOfPlayers);

    }

    public override void OnSceneLoadDone(NetworkRunner runner)
    {
        base.OnSceneLoadDone(runner);
        Debug.Log("Scene loaded successfully.");
    }

    public override void OnSceneLoadStart(NetworkRunner runner)
    {
        base.OnSceneLoadStart(runner);
        Debug.Log("Scene load started.");
    }
    public void EndSession()
    {
        if (networkRunner.IsRunning)
        {
            networkRunner.Shutdown();
        }
    }


}