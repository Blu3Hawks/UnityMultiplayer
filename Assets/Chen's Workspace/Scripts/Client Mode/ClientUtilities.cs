using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class ClientUtilities : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LobbyManager _lobbyManager;
    [SerializeField] private string _lobbyName = "HostModeLobby";
    [SerializeField] private GameObject _joiningLobbyObject;
    [SerializeField] private GameObject _activeSessionObject;

    private void Awake()
    {
        if (_joiningLobbyObject) _joiningLobbyObject.SetActive(false);
        StartCoroutine(JoinSesisonAndShowRooms());
    }

    private IEnumerator JoinSesisonAndShowRooms()
    {
        bool ok = false;
        yield return _lobbyManager.JoinLobbyCoroutine(_lobbyName, success => ok = success);
        if (_joiningLobbyObject)
        {
            _joiningLobbyObject.SetActive(ok);
        }
    }

    public void ShowActiveSessionUI(bool joinedOk)
    {
        if (!joinedOk) return;
        if (_joiningLobbyObject) _joiningLobbyObject.SetActive(false);
        if (_activeSessionObject) _activeSessionObject.SetActive(true);
    }
}
