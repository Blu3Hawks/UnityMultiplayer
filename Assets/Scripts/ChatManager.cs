using Fusion;
using UI;
using UnityEngine;

public class ChatManager : NetworkBehaviour {
    [SerializeField] private EmoteUiManager emoteUiManager;
    
    // Call this from any client to send an emote to exactly one player.
    public void SendEmoteToPlayer(EmoteType emoteType, PlayerRef target)
    {
        // Pass both the emote ID and who it's for,
        // plus the sender (InputAuthority) to know who sent it.
        RpcReceiveEmote(emoteType, target, Object.InputAuthority);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcReceiveEmote(EmoteType emoteType, PlayerRef target, PlayerRef sender)
    {
        // Only the intended player should handle this
        if (Runner.LocalPlayer != target)
            return;

        // Display it in your UI:
        emoteUiManager.ShowEmote(emoteType, sender);
    }
}

public enum EmoteType {
    Smile,
    Pressured,
    Cry,
    Clown,
    Dead,
    Cool,
}