using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostMode_Utilities : MonoBehaviour
{
    [Header("Lobby & Sessions")]
    [SerializeField] private string _customLobbyName = "HostModeLobby";
    [SerializeField] private List<string> _sessionNames = new List<string>() { "Room_A", "Room_B", "Room_C" };
    [SerializeField] private int _maxPlayers = 5;

    [Header("Prefabs")]
    [Tooltip("A prefab that already has NetworkRunner + NetworkSceneManagerDefault (and optionally RoomVotingServer).")]
    [SerializeField] private GameObject _runnerPrefab;
    [Tooltip("Networked prefab with NetworkObject + RoomState script (registered in Network Project Config).")]
    [SerializeField] private NetworkObject _roomStatePrefab;

    private readonly List<NetworkRunner> _networkRunners = new List<NetworkRunner>();

    private async void Start()
    {
        await CreateAllSessions();
    }

    private async Task CreateAllSessions()
    {
        foreach (string name in _sessionNames)
        {
            var runnerGO = Instantiate(_runnerPrefab, transform);
            runnerGO.name = $"Runner_{name}";

            var runner = runnerGO.GetComponent<NetworkRunner>();
            var sceneMgr = runnerGO.GetComponent<NetworkSceneManagerDefault>();
            runner.ProvideInput = true;
            _networkRunners.Add(runner);

            var args = new StartGameArgs
            {
                GameMode = GameMode.Server,
                SessionName = name,
                CustomLobbyName = _customLobbyName,
                SceneManager = sceneMgr,
                PlayerCount = _maxPlayers
            };

            var result = await runner.StartGame(args);
            if (!result.Ok)
            {
                Debug.LogError($"[SERVER] Failed to start session '{name}': {result.ShutdownReason}");
                continue;
            }

            Debug.Log($"[SERVER] Started session '{name}' in lobby '{_customLobbyName}'.");

            if (_roomStatePrefab)
            {
                runner.Spawn(_roomStatePrefab);
                Debug.Log($"[SERVER] Spawned RoomState in session '{name}'.");
            }
            else
            {
                Debug.LogWarning($"[SERVER] RoomState prefab not assigned; clients won�t have room voting/state.");
            }
        }
    }
}
