using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class SessionData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI activePlayers;

    public event UnityAction<SessionInfo> OnSessionSelected;

    private SessionInfo current;

    public SessionInfo CurrentSessionInfo => current;
    public string SessionName { get; private set; }



    public void InitializeLobby(SessionInfo session)
    {
        SessionName = session.Name;
        current = session;
        this.lobbyName.SetText($"{session.Name}");
        this.activePlayers.SetText($"Players: {session.PlayerCount}/{session.MaxPlayers}");
    }

    public void SessionSelectedPressed()
    {
        OnSessionSelected?.Invoke(current);
    }
}