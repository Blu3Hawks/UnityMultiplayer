using Fusion;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HostMode_Utilities : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LobbyManager _lobbyManager;
    [SerializeField] private NetworkRunner _networkRunner;
    [Header("Settings")]
    [SerializeField] private string _hostModeLobbyName = "HostModeLobby";
    [SerializeField] private int _maxPlayerAmount = 5;


    [Header("Session Settings")]
    [SerializeField] private List<string> _sessionNames;


    private void OnEnable()
    {
        _lobbyManager.OnSessionStarted += CreateSessions;
    }


    private void Start()
    {
        _lobbyManager.JoinLobby(_hostModeLobbyName);
        _lobbyManager.SetMaxAmountOfPlayers(_maxPlayerAmount);


        // _lobbyManager.StartSession(_sessionName1);
    }

    private void CreateSessions()
    {
        foreach (string sessionName in _sessionNames)
        {
            _lobbyManager.StartSession(sessionName);
            Debug.Log($"Session '{sessionName}' created.");
        }

    }

}
