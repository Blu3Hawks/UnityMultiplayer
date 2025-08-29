using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomState : NetworkBehaviour
{
    [Networked] public int ReadyCount { get; set; }
    [Networked] public int TotalClients { get; set; }
    [Networked] public bool VotingOpen { get; set; }
    [Networked] public bool GameStarting { get; set; }
    public static RoomState Current { get; private set; }
    public static event Action<RoomState> CurrentChanged;

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

        ReadyCount = _ready.Count;
        TryStartIfThresholdMet();
    }
    public override void Spawned()
    {
        Current = this;
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
        if (!Runner.IsServer || GameStarting) return;

        if (TotalClients <= 0) return;

        bool ok = requireAllReady
            ? (ReadyCount >= TotalClients)
            : (ReadyCount >= Mathf.CeilToInt(TotalClients * readyRatio));

        if (ok)
        {
            Debug.Log("we start the game !");
            GameStarting = true;
            VotingOpen = false;
            Runner.LoadScene("TestingScene");
        }
        else
        {
            VotingOpen = true;
        }
    }
    public void ServerOnPlayerJoined(int totalClients)
    {
        if (!Runner.IsServer) return;
        TotalClients = totalClients;
        ReadyCount = _ready.Count;
    }

    public void ServerOnPlayerLeft(PlayerRef player, int totalClients)
    {
        if (!Runner.IsServer) return;
        _ready.Remove(player);
        TotalClients = totalClients;
        ReadyCount = _ready.Count;
    }
}
