using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

namespace UI
{
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

            if (lobbyUI != null)
                lobbyUI.SetActive(false);
        }
        private void OnSessionStart()
        {
            activeSessionUI.SetActive(true);
            createSessionUI.SetActive(false);
            if (lobbyUI != null)
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

            //create a dictionary to map session names to SessionData objects
            Dictionary<string, SessionData> sessionDataByName = new Dictionary<string, SessionData>();
            foreach (var sessionData in currentSessions)
            {
                if (!string.IsNullOrEmpty(sessionData.SessionName))
                    sessionDataByName[sessionData.SessionName] = sessionData;
            }

            //track the sessions by their names via hashset
            HashSet<string> visibleSessionNames = new HashSet<string>();

            //then foreach try to get the value by the name, if it exists, update it, otherwise instantiate a new one
            foreach (var session in sessions)
            {
                visibleSessionNames.Add(session.Name);

                if (sessionDataByName.TryGetValue(session.Name, out var sessionData))
                {
                    sessionData.InitializeLobby(session);
                    sessionData.gameObject.SetActive(true);
                }
                else
                {
                    SessionData newSession = Instantiate(sessionDataPrefab, sessionParent);
                    newSession.InitializeLobby(session);
                    newSession.OnSessionSelected += SessionSelected;
                    currentSessions.Add(newSession);
                    newSession.gameObject.SetActive(true);
                }
            }

            //and after that, disable all session data objects that are not in the list
            foreach (var sessionData in currentSessions)
            {
                if (!visibleSessionNames.Contains(sessionData.SessionName))
                {
                    sessionData.gameObject.SetActive(false);
                }
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
