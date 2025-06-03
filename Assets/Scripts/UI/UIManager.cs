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

    [Header("UI References")]
    [SerializeField] private GameObject activeSessionUI;
    [SerializeField] private GameObject createSessionUI;

    private List<SessionData> currentSessions = new List<SessionData>();

    public void PlayerConnection(PlayerRef player, bool Joined)
    {
        playerJoinedText.SetText($"Player: {player.PlayerId} {(Joined ? "Joined" : "Left")} the lobby");
        UpdateUI();
        // float timer = 0;
        // while (timer < 1) 
        // {
        //     timer += Time.deltaTime;
        // }
        // playerJoinedText.SetText("");
        
    }

    private void OnSessionStart(List<SessionInfo> sessionInfo)
    {
        createSessionUI.SetActive(false);
        activeSessionUI.SetActive(true);
    }


    private void UpdateUI()
    {
        amountOfPlayers.SetText($"Current Amount Of Players: {lobbyManager.AmountOfPlayers}");
    }

    public void UpdateSessionList(List<SessionInfo> sessions)
    {
        UpdateUI();
        Debug.Log($"Session count: {sessions.Count}");
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


    private void OnEnable()
    {
        lobbyManager.onSessionListUpdated += UpdateSessionList;
        lobbyManager.onPlayersListChanged += PlayerConnection;
        lobbyManager.onSessionListUpdated += OnSessionStart;
        UpdateUI();
    }

    private void OnDisable()
    {
        lobbyManager.onSessionListUpdated -= UpdateSessionList;
        lobbyManager.onPlayersListChanged -= PlayerConnection;
    }

}
