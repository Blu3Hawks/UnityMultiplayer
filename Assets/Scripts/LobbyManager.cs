using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // Events
    public event UnityAction<List<SessionInfo>> onSessionListUpdated;
    public event UnityAction<PlayerRef, bool> onPlayersListChanged;
    public event UnityAction onSessionShutdown;

    public event UnityAction OnLobbyEntered;

    public event UnityAction OnSessionStarted;
    public event UnityAction<bool> OnHidingSession;

    [Header("References")]
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private GameObject readyManagerGeneric;
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private Button startGameButton;
    //need to add an int of max players to the lobby, so that it can be set through the UI
    [SerializeField] private TMP_Dropdown amountOfPlayersDropdown;

    public ReadyManager readyManagerInstance;

    // Session list
    private List<SessionInfo> _sessionsList = new();

    //private variables
    private int amountOfPlayers;
    private int maxAmountOfPlayers = 5;

    private List<PlayerRef> playersInLobby = new List<PlayerRef>(); 

    public List<PlayerRef> PlayersInLobby => playersInLobby;

    private string currentLobby;
    //properties
    public int AmountOfPlayers { get { return networkRunner.SessionInfo.PlayerCount; } }
    public int MaxAmountOfPlayers { get { return maxAmountOfPlayers; } }

    //scene const names
    public const string GAME_SCENE_NAME = "TestingScene";

    public const string LOBBY_SCENE_NAME = "MainMenu";


    //static reference
    public static LobbyManager Instance { get; private set; }

    public async void StartSession(string sessionName)
    {
        //Debug.Log(lobbyName.text);
        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            OnGameStarted = OnGameStarted,
            CustomLobbyName = currentLobby,
            PlayerCount = maxAmountOfPlayers
        });
        OnSessionStarted?.Invoke();
    }
    void Awake()
    {
        networkRunner.AddCallbacks(this);
        //onSessionShutdown += HandleSessionShutdown;
    }

   // private void HandleSessionShutdown()
    //{
      //  UnityEngine.SceneManagement.SceneManager.LoadScene(LOBBY_SCENE_NAME);
    //}

    public void StartMatch()
    {
        if (networkRunner.IsSceneAuthority)
        {
            networkRunner.LoadScene(GAME_SCENE_NAME);
        }
    }

    public void StartSessionWithInput()
    {
        StartSession(lobbyName.text);
    }

    private void OnGameStarted(NetworkRunner obj)
    {
        // Debug.Log("Game Started + Chen HaHomo");
        if (networkRunner.IsSharedModeMasterClient)
            networkRunner.Spawn(readyManagerGeneric);
        amountOfPlayers++;

        onSessionListUpdated?.Invoke(_sessionsList);
        //Debug.Log(amountOfPlayers);

        startGameButton.interactable = false;
        if (networkRunner.IsSceneAuthority)
        {
            startGameButton.interactable = true;
            startGameButton.onClick.AddListener(StartMatch);
        }

    }

    public IEnumerator JoinLobbyCoroutine(string lobbyId, System.Action<bool> onDone = null)
    {
        currentLobby = lobbyId;

        var task = networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyId);
        // wait until the async task finishes
        yield return new WaitUntil(() => task.IsCompleted);

        var ok = task.IsCompletedSuccessfully && task.Result.Ok;
        if (ok) OnLobbyEntered?.Invoke();
        else Debug.LogError($"Failed to join lobby: {task.Result.ShutdownReason}");

        onDone?.Invoke(ok);
    }

    public async void JoinLobby(string LobbyID)
    {
        currentLobby = LobbyID;
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, LobbyID);

        //just check if it's not okay, so we can return early
        if (!result.Ok)
        {
            Debug.LogError($"Failed to join lobby: {result.ShutdownReason}");
            return;
        }

        //now we want to check if the lobby is open or not
        //we will iterate through the sessions list and check if the lobby is open. 
        foreach (var session in _sessionsList)
        {
            if (session.Name == LobbyID)
            {
                if (!session.IsOpen)
                {
                    Debug.Log("Lobby is not open");
                    return;
                }
            }
        }

        //check if the lobby is full, if not then join
        if (amountOfPlayers >= MaxAmountOfPlayers)
        {
            Debug.Log("Can't join the lobby, no space");
            return;
        }

        if (result.Ok)
        {
            Debug.Log("Lobby Joined Successfully");
            OnLobbyEntered?.Invoke();
        }

    }

    public void PressHideSession()
    {
        networkRunner.SessionInfo.IsVisible = !networkRunner.SessionInfo.IsVisible;
        networkRunner.SessionInfo.IsOpen = !networkRunner.SessionInfo.IsOpen;
        OnHidingSession?.Invoke(networkRunner.SessionInfo.IsVisible);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        _sessionsList = sessionList;
        Debug.Log($"Session count: {_sessionsList.Count}");
        onSessionListUpdated?.Invoke(_sessionsList);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        amountOfPlayers = runner.SessionInfo.PlayerCount;
        if (!playersInLobby.Contains(player)) playersInLobby.Add(player);
        onPlayersListChanged?.Invoke(player, true); // When player joined - invoke with true bool
        //Debug.Log($"playercount: {runner.SessionInfo?.PlayerCount}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        amountOfPlayers--;
        if (playersInLobby.Contains(player)) playersInLobby.Remove(player);
        onPlayersListChanged?.Invoke(player, false); // When player left - invoke with false bool
        Debug.Log(amountOfPlayers);

    }

    public void SetMaxAmountOfPlayers()
    {
        maxAmountOfPlayers = amountOfPlayersDropdown.value + 2;
        Debug.Log(maxAmountOfPlayers);
    }

    public void SetMaxAmountOfPlayers(int maxAmountOfPlayers)
    {
        this.maxAmountOfPlayers = maxAmountOfPlayers;
    }


    public async Task<bool> JoinSessionAsClientAsync(string sessionName)
    {
        if (!networkRunner) networkRunner = GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
        var sceneMgr = GetComponent<NetworkSceneManagerDefault>() ?? gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,//cuz we all clients here
            SessionName = sessionName,
            SceneManager = sceneMgr
        });

        if (!result.Ok)
            Debug.LogError($"[CLIENT] Join failed: {result.ShutdownReason}");

        return result.Ok;
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

        Debug.Log("Scene loaded successfully.");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load started.");
    }

    public void EndSession()
    {

        if (networkRunner.IsRunning)
        {
            networkRunner.Shutdown();
        }

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        onSessionShutdown.Invoke();
        runner.Shutdown();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }
}