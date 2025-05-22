using Fusion;
using TMPro;
using UnityEngine;


public class SessionData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI activePlayers;

    public void InitializeLobby(SessionInfo session)
    {
        this.lobbyName.SetText($"{session.Name}");
        this.activePlayers.SetText($"Players: {session.PlayerCount}/{session.MaxPlayers}");
    }
}