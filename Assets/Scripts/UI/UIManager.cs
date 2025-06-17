using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

namespace UI {
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
        [SerializeField] private GameObject lobbyUI;
        
        private List<SessionData> currentSessions = new List<SessionData>();

        private void PlayerConnection(PlayerRef player, bool joined)
        {
            playerJoinedText.SetText($"Player: {player.PlayerId} {(joined ? "Joined" : "Left")} the lobby");
            UpdateUI();
            // float timer = 0;
            // while (timer < 1) 
            // {
            //     timer += Time.deltaTime;
            // }
            // playerJoinedText.SetText("");

        }

        private void OnLobbyJoined()
        {
            createSessionUI.SetActive(true);
            lobbyUI.SetActive(false);
        }
        private void OnSessionStart()
        {
            activeSessionUI.SetActive(true);
            createSessionUI.SetActive(false);
            lobbyUI.SetActive(false);
        }

        private void OnSessionShutDown()
        {
            activeSessionUI.SetActive(false);
            createSessionUI.SetActive(true);
        }

        private void UpdateUI()
        {
            Debug.Log(lobbyManager.MaxAmountOfPlayers);
            amountOfPlayers.SetText($"Current Amount Of Players: {lobbyManager.AmountOfPlayers} / {lobbyManager.MaxAmountOfPlayers}");
        }

        private void UpdateSessionList(List<SessionInfo> sessions)
        {
            UpdateUI();
            Debug.Log($"Session count in ui manager: {sessions.Count}");
            for (int i = 0; i < sessions.Count; i++)
            {
                if (i >= currentSessions.Count)
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

            for (int i = sessions.Count; i < currentSessions.Count; i++)
            {
                Destroy(currentSessions[i].gameObject);
            }
        }

        private void SessionSelected(SessionInfo session)
        {
            lobbyManager.StartSession(session.Name);
        }

        private void OnEnable()
        {
            lobbyManager.onSessionShutdown += OnSessionShutDown;
            lobbyManager.onSessionListUpdated += UpdateSessionList;
            lobbyManager.onPlayersListChanged += PlayerConnection;
            lobbyManager.OnLobbyEntered += OnLobbyJoined;
            lobbyManager.OnSessionStarted += OnSessionStart;
            UpdateUI();
        }

        private void OnDisable()
        {
            lobbyManager.onSessionListUpdated -= UpdateSessionList;
            lobbyManager.onPlayersListChanged -= PlayerConnection;
        }

    }
}
