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
    [SerializeField] private TMP_Dropdown amountOfPlayersDropdown;
    [SerializeField] private GameObject _activeSessionObject;

    public ReadyManager readyManagerInstance;

    // Session list
    private List<SessionInfo> _sessionsList = new();

    // Private variables
    private int amountOfPlayers;
    private int maxAmountOfPlayers = 5;
    private PlayerRef? _startingPlayerRef = null;
    private Transform _runnerRoot;
    private List<PlayerRef> playersInLobby = new List<PlayerRef>();
    public List<PlayerRef> PlayersInLobby => playersInLobby;

    private string currentLobby;

    // Properties
    public int AmountOfPlayers => networkRunner.SessionInfo.PlayerCount;
    public int MaxAmountOfPlayers => maxAmountOfPlayers;

    // Scene names
    public const string GAME_SCENE_NAME = "TestingScene";
    public const string LOBBY_SCENE_NAME = "MainMenu";

    // Static reference
    public static LobbyManager Instance { get; private set; }

    public async void StartSession(string sessionName)
    {
        if (!networkRunner || networkRunner.gameObject == this.gameObject)
            networkRunner = CreateRunnerChild("ServerRunner", provideInput: false);

        var sceneMgr = networkRunner.GetComponent<NetworkSceneManagerDefault>() ??
                       networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Server,
            SessionName = sessionName,
            OnGameStarted = OnGameStarted,
            CustomLobbyName = currentLobby,
            PlayerCount = maxAmountOfPlayers,
            IsOpen = true,
            IsVisible = true,
            SceneManager = sceneMgr
        });

        OnSessionStarted?.Invoke();
    }

    void Awake()
    {
        networkRunner.AddCallbacks(this);
    }

    public void StartMatch()
    {
        networkRunner.LoadScene(GAME_SCENE_NAME);
    }

    public void StartSessionWithInput()
    {
        StartSession(lobbyName.text);
    }

    private void OnGameStarted(NetworkRunner obj)
    {
        if (networkRunner.IsServer)
            networkRunner.Spawn(readyManagerGeneric);

        amountOfPlayers++;
        onSessionListUpdated?.Invoke(_sessionsList);

        startGameButton.interactable = false;

        if (networkRunner.IsSceneAuthority)
            startGameButton.interactable = true;
    }

    public IEnumerator JoinLobbyCoroutine(string lobbyId, System.Action<bool> onDone = null)
    {
        if (!networkRunner || networkRunner.gameObject == this.gameObject)
            networkRunner = CreateRunnerChild("ClientRunner_Lobby", provideInput: true);

        currentLobby = lobbyId;

        var task = networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyId);
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

        onSessionListUpdated?.Invoke(_sessionsList);

        if (!result.Ok)
        {
            Debug.LogError($"Failed to join lobby: {result.ShutdownReason}");
            return;
        }

        onSessionListUpdated?.Invoke(_sessionsList);

        foreach (var session in _sessionsList)
        {
            if (session.Name == LobbyID && !session.IsOpen)
            {
                Debug.Log("Lobby is not open");
                return;
            }
        }

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

        if (!playersInLobby.Contains(player))
            playersInLobby.Add(player);

        if (_startingPlayerRef == null)
        {
            _startingPlayerRef = player;
            Debug.Log($"First player is now {player.PlayerId}");
            UpdateStartButtonAuthority();
        }

        onPlayersListChanged?.Invoke(player, true);
        onSessionListUpdated?.Invoke(_sessionsList);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        amountOfPlayers--;

        if (playersInLobby.Contains(player))
            playersInLobby.Remove(player);

        if (_startingPlayerRef == player)
        {
            _startingPlayerRef = playersInLobby.Count > 0 ? playersInLobby[0] : (PlayerRef?)null;
            Debug.Log(_startingPlayerRef.HasValue
                ? $"First player reassigned to {_startingPlayerRef.Value.PlayerId}"
                : "No players left, first player cleared.");
            UpdateStartButtonAuthority();
        }

        onPlayersListChanged?.Invoke(player, false);
        onSessionListUpdated?.Invoke(_sessionsList);
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
        if (!networkRunner || networkRunner.gameObject == this.gameObject)
            networkRunner = CreateRunnerChild("ClientRunner", provideInput: true);

        var sceneMgr = networkRunner.GetComponent<NetworkSceneManagerDefault>() ??
                       networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
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
            networkRunner.Shutdown();
    }

    public async void OnLeaveButtonPressed()
    {
        await LeaveSessionAsync(rejoinLobby: true);
        _activeSessionObject.SetActive(false);
        onSessionListUpdated?.Invoke(_sessionsList);
    }

    public async Task<bool> LeaveSessionAsync(bool rejoinLobby = true)
    {
        if (networkRunner && networkRunner.IsRunning)
            await networkRunner.Shutdown();

        DestroyRunnerIfChild();

        if (rejoinLobby && !string.IsNullOrEmpty(currentLobby))
        {
            networkRunner = CreateRunnerChild("ClientRunner_Lobby", provideInput: true);
            var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, currentLobby);

            if (!result.Ok)
            {
                Debug.LogError($"Failed to re-join lobby '{currentLobby}': {result.ShutdownReason}");
                return false;
            }

            OnLobbyEntered?.Invoke();
            onSessionListUpdated?.Invoke(_sessionsList);
        }

        return true;
    }

    private void EnsureRunnerRoot()
    {
        if (_runnerRoot == null)
        {
            var root = new GameObject("RunnerRoot");
            root.transform.SetParent(transform, false);
            _runnerRoot = root.transform;
        }
    }

    private NetworkRunner CreateRunnerChild(string name, bool provideInput)
    {
        EnsureRunnerRoot();
        var go = new GameObject(name);
        go.transform.SetParent(_runnerRoot, false);
        var runner = go.AddComponent<NetworkRunner>();
        runner.AddCallbacks(this);
        runner.ProvideInput = provideInput;
        return runner;
    }

    private void UpdateStartButtonAuthority()
    {
        if (startGameButton == null) return;

        if (_startingPlayerRef.HasValue && networkRunner.LocalPlayer == _startingPlayerRef.Value)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    private void DestroyRunnerIfChild()
    {
        if (networkRunner)
        {
            var go = networkRunner.gameObject;
            networkRunner = null;
            if (go) Destroy(go);
        }
    }

    // INetworkRunnerCallbacks (empty ones left as stubs)
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (runner.IsServer) onSessionShutdown?.Invoke();
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
}
