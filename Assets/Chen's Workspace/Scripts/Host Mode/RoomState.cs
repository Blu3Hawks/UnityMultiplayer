using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using Game_Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomState : NetworkBehaviour
{
    [Networked] public int ReadyCount { get; set; }
    [Networked] public int TotalClients { get; set; } = 2;
    [Networked] public bool VotingOpen { get; set; }
    [Networked] public bool GameStarting { get; set; }
    public static RoomState Current { get; private set; }

    private Camera oldCam;
    public static event Action<RoomState> CurrentChanged;

    public static event UnityAction OnRoomStarted;

    [Header("Voting Settings (server-only)")]
    [SerializeField] public bool requireAllReady = true;
    [SerializeField, Range(0.5f, 1f)] public float readyRatio = 1f;

    private readonly HashSet<PlayerRef> _ready = new HashSet<PlayerRef>();

    public void ServerSetTotals(int totalClients)
    {
        if (!Runner || !Runner.IsServer) return;
        TotalClients = totalClients;
        ReadyCount = _ready.Count;
    }

    public void ServerClearReady(PlayerRef player)
    {
        if (!Runner || !Runner.IsServer) return;
        _ready.Remove(player);
        ReadyCount = _ready.Count;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetReady(bool ready, RpcInfo info = default)
    {
        if (!Runner.IsServer) return;

        var player = info.Source;
        if (ready) _ready.Add(player);
        else _ready.Remove(player);
        Debug.Log("Player clicked ready: " + player.PlayerId + $" {_ready.Count}");
        ReadyCount = _ready.Count;
        TryStartIfThresholdMet();
    }
    public override void Spawned()
    {
        Current = this;
        oldCam = Camera.main;
        CurrentChanged?.Invoke(this);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Current == this)
        {
            Current = null;
            CurrentChanged?.Invoke(null);
        }
    }
    private void TryStartIfThresholdMet() 
    {
        Debug.Log("Checking if server");
        if (!Runner.IsServer || GameStarting) return;

        // Debug.Log("Checking if clients exists");
        // if (TotalClients <= 0) return;

        Debug.Log("Checking if ready");
        bool ok = (ReadyCount >= Runner.ActivePlayers.Count());

        if (ok)
        {
            Debug.Log("we start the game !");
            GameStarting = true;
            VotingOpen = false;
            Runner.LoadScene("GameReady", LoadSceneMode.Additive);
            RpcRoomStarted();
        }
        else
        {
            Debug.Log("Not ready yet");
            VotingOpen = true;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RpcRoomStarted()
    {
        GameEvents.RaiseRoomStarted();
    }
    public void ServerOnPlayerJoined(int totalClients)
    {
        if (!Runner.IsServer) return;
        TotalClients = totalClients;
        ReadyCount = _ready.Count;
    }

    public void ServerOnPlayerLeft(PlayerRef player)
    {
        if (!Runner.IsServer) return;
        _ready.Remove(player);
        TotalClients -= 1;
        ReadyCount = _ready.Count;
    }
}
