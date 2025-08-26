using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class SessionData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI activePlayers;
    [SerializeField] private LobbyManager _lobbyManager;
    [SerializeField] private ClientUtilities _clientUtilities;

    public event UnityAction<SessionInfo> OnSessionSelected;

    private SessionInfo current;

    public SessionInfo CurrentSessionInfo => current;
    public string SessionName { get; private set; }



    public void InitializeLobby(SessionInfo session, LobbyManager lobbyManager, ClientUtilities clientUtilities)
    {
        SessionName = session.Name;
        current = session;
        _lobbyManager = lobbyManager;
        _clientUtilities = clientUtilities;
        this.lobbyName.SetText($"{session.Name}");
        this.activePlayers.SetText($"Players: {session.PlayerCount}/{session.MaxPlayers}");
    }

    public async void SessionSelected(SessionInfo session)
    {
        bool ok = await _lobbyManager.JoinSessionAsClientAsync(session.Name);
        if (ok)
        {
            _clientUtilities.ShowActiveSessionUI(true);
        }
    }
    public async void ConnectToSession()
    {
        bool ok = await _lobbyManager.JoinSessionAsClientAsync(SessionName);
        if (_clientUtilities) _clientUtilities.ShowActiveSessionUI(ok);
    }
}