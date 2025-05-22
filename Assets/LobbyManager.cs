using Fusion;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunner;

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
}
