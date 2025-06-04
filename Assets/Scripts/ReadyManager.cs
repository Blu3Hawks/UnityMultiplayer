using Fusion;
using System;
using System.Diagnostics;

public class ReadyManager : NetworkBehaviour
{

    public int readyCounter = 0;
    [Rpc]
    public void SetReadyRPC(RpcInfo info = default)
    {
        Console.WriteLine("SetReadyRPC called by " + info.Source.PlayerId);
        readyCounter++;
    }

    public override void Spawned()
    {
        base.Spawned();
        LobbyManager.Instance.readyManagerInstance = this;
    }
}