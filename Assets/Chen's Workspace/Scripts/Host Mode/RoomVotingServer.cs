using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomVotingServer : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Prefabs / Settings")]
    [SerializeField] private RoomState roomStatePrefab;
    [SerializeField] private string gameSceneName = "TestingScene";

    private NetworkRunner _runner;
    private RoomState _roomState;

    private void Awake()
    {
        _runner = GetComponent<NetworkRunner>();
        if (_runner) _runner.AddCallbacks(this);
    }

    private void Start()
    {
        if (_runner && _runner.IsServer && roomStatePrefab)
        {
            _roomState = _runner.Spawn(roomStatePrefab);
            _roomState.VotingOpen = true;
            _roomState.GameStarting = false;
            PushTotals();
        }
    }

    private void PushTotals()
    {
        if (_roomState == null || !_runner) return;
        _roomState.ServerSetTotals(_runner.ActivePlayers.Count());
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;
        if (_roomState)
        {
            _roomState.ServerOnPlayerJoined(runner.ActivePlayers.Count());
            PushTotals();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;
        if (_roomState)
        {
            _roomState.ServerClearReady(player);
            _roomState.ServerOnPlayerLeft(player, runner.ActivePlayers.Count());
            PushTotals();
        }
    }

    public void OnSessionListUpdated(NetworkRunner r, List<SessionInfo> s) { }
    public void OnShutdown(NetworkRunner r, ShutdownReason reason) { }
    public void OnConnectedToServer(NetworkRunner r) { }
    public void OnDisconnectedFromServer(NetworkRunner r, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner r, NetAddress remote, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner r, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner r, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner r, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner r, PlayerRef player, ReliableKey key, float progress) { }
    public void OnUserSimulationMessage(NetworkRunner r, SimulationMessagePtr message) { }
    public void OnObjectEnterAOI(NetworkRunner r, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner r, NetworkObject obj, PlayerRef player) { }
    public void OnCustomAuthenticationResponse(NetworkRunner r, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner r, HostMigrationToken token) { }
    public void OnSceneLoadStart(NetworkRunner r) { }
    public void OnSceneLoadDone(NetworkRunner r) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new System.NotImplementedException();
    }
}
