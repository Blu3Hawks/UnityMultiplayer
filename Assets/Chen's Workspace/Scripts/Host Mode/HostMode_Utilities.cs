using Fusion;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostMode_Utilities : MonoBehaviour
{
    [Header("Lobby & Sessions")]
    [SerializeField] private string _customLobbyName = "HostModeLobby";
    [SerializeField] private List<string> _sessionNames = new List<string>() { "Room_A", "Room_B", "Room_C" };
    [SerializeField] private int _maxPlayers = 5;
    [SerializeField] private PlayerRef _firstPlayerRef; //make it a reference, use it to store the first player, then use it to know who can start the game with the UI button

    private readonly List<NetworkRunner> _networkRunners = new List<NetworkRunner>();

    private async void Start()
    {
        await CreateAllSessions();
    }

    private async Task CreateAllSessions()
    {
        foreach (string name in _sessionNames)
        {
            GameObject runnerGameObject = new GameObject($"Runner_{name}");
            runnerGameObject.transform.SetParent(transform);

            NetworkRunner networkRunner = runnerGameObject.AddComponent<NetworkRunner>();
            NetworkSceneManagerDefault sceneManager = runnerGameObject.AddComponent<NetworkSceneManagerDefault>();

            networkRunner.ProvideInput = false;
            _networkRunners.Add(networkRunner);

            StartGameArgs args = new StartGameArgs
            {
                GameMode = GameMode.Server,
                SessionName = name,
                CustomLobbyName = _customLobbyName,
                SceneManager = sceneManager,
                PlayerCount = _maxPlayers
            };

            var result = await networkRunner.StartGame(args);
            if (!result.Ok)
            {
                Debug.LogError($"Failed to start session '{name}': {result.ShutdownReason}");
            }
            else
            {
                Debug.Log($"Started session '{name}' in lobby '{_customLobbyName}'.");
            }
        }
    }
}
