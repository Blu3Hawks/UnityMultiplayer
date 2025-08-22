using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class SessionData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI activePlayers;
    [SerializeField] private LobbyManager _lobbyManager;
    [SerializeField] private ClientUtilities _clientUI; 

    public event UnityAction<SessionInfo> OnSessionSelected;

    private SessionInfo current;

    public SessionInfo CurrentSessionInfo => current;
    public string SessionName { get; private set; }



    public void InitializeLobby(SessionInfo session, LobbyManager lobbyManager, ClientUtilities clientUtilities)
    {
        SessionName = session.Name;
        current = session;
        _lobbyManager = lobbyManager;
        _clientUI = clientUtilities;
        this.lobbyName.SetText($"{session.Name}");
        this.activePlayers.SetText($"Players: {session.PlayerCount - 1}/{session.MaxPlayers - 1}");
    }

    public void SessionSelectedPressed()
    {
        OnSessionSelected?.Invoke(current);
    }

    public async void ConnectToSession()
    {
        bool ok = await _lobbyManager.JoinSessionAsClientAsync(SessionName);
        if (_clientUI) _clientUI.ShowActiveSessionUI(ok);
    }
}