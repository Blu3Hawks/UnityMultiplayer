using Fusion;
using UI;
using UnityEngine;

public class ChatManager : NetworkBehaviour {
    [SerializeField] private EmoteUiManager emoteUiManager;
    
    // Call this from ANY client to send an emote to exactly one player.
    public void SendEmoteToPlayer(EmoteType emoteType, PlayerRef target)
    {
        // pass both the emote ID and who it's for,
        // plus the sender (InputAuthority) so you know who sent it.
        RpcReceiveEmote(emoteType, target, Object.InputAuthority);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcReceiveEmote(EmoteType emoteType, PlayerRef target, PlayerRef sender)
    {
        // Only the intended recipient should handle this!
        if (Runner.LocalPlayer != target)
            return;

        // Now display it in your UI:
        // e.g. EmoteUI.Instance.ShowEmote(emote, sender);
        emoteUiManager.ShowEmote(emoteType, sender);
        //Debug.Log($"💬 Emote {emoteId} ⬅️ from {sender}");
    }
}

public enum EmoteType {
    
}