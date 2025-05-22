using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerJoinedText;

    [SerializeField] private LobbyManager lobbyManager;

    [SerializeField] private SessionData sessionDataPrefab;

    [SerializeField] private RectTransform sessionParent;

    private List<SessionData> currentSessions = new List<SessionData>();


    public void PlayerConnected(PlayerRef player)
    {
        playerJoinedText.SetText($"Player: {player.PlayerId} Joined the lobby");
    }

    public void UpdateSessionList(List<SessionInfo> sessions)
    {
        for (int i = 0; i < sessions.Count; i++)
        {
            if (i < currentSessions.Count)
            {
                SessionData newSession = Instantiate(sessionDataPrefab, sessionParent);
                newSession.InitializeLobby(sessions[i]);
                currentSessions.Add(newSession);
            }
            else
            {
                currentSessions[i].InitializeLobby(sessions[i]);
            }
        }
    }
}
