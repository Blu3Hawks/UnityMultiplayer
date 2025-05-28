using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerJoinedText;

    [SerializeField] private TextMeshProUGUI amountOfPlayers;

    [SerializeField] private LobbyManager lobbyManager;

    [SerializeField] private SessionData sessionDataPrefab;

    [SerializeField] private RectTransform sessionParent;

    private List<SessionData> currentSessions = new List<SessionData>();

    void Start()
    {
        lobbyManager.onSessionListUpdated += UpdateSessionList;
        lobbyManager.onPlayersListChanged += PlayerConnection;
    }

    public void PlayerConnection(PlayerRef player, bool Joined)
    {
        playerJoinedText.SetText($"Player: {player.PlayerId} {(Joined ? "Joined" : "Left")} the lobby");
        UpdateUI();
        float timer = 0;
        while (timer < 1) 
        {
            timer += Time.deltaTime;
        }
        playerJoinedText.SetText("");
        
    }

    private void UpdateUI()
    {
        amountOfPlayers.SetText($"Current Amount Of Players: {lobbyManager.AmountOfPlayers}");
    }

    public void UpdateSessionList(List<SessionInfo> sessions)
    {
        UpdateUI();
        for (int i = 0; i < sessions.Count; i++)
        {
            if (i < currentSessions.Count)
            {
                SessionData newSession = Instantiate(sessionDataPrefab, sessionParent);
                newSession.InitializeLobby(sessions[i]);
                currentSessions.Add(newSession);
                newSession.OnSessionSelected += SessionSelected;
            }
            else
            {
                currentSessions[i].InitializeLobby(sessions[i]);
            }
        }
    }

    private void SessionSelected(SessionInfo session)
    {

    }
}
