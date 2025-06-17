using System.Linq;
using Fusion;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour {
    [SerializeField] private EmoteUiManager emoteUiManager;

    
    [SerializeField] private TMP_Dropdown playersDropdown;

    void Start()
    {
        LobbyManager.Instance.onPlayersListChanged += HandlePlayerList;
        UpdateDropdown();
    }

    private void HandlePlayerList(PlayerRef playerRef, bool joined){
        UpdateDropdown();
    }
    private void UpdateDropdown(){
        playersDropdown.ClearOptions();
        playersDropdown.AddOptions(LobbyManager.Instance.PlayersInLobby.Select(player => player.PlayerId.ToString()).ToList());
    }

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