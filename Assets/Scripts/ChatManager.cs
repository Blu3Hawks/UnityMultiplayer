using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour {
    [SerializeField] private EmoteUiManager emoteUiManager;

    
    [SerializeField] private TMP_Dropdown playersDropdown;

    [SerializeField] private List<EmoteButton> emoteButtons;

    


    public override void Spawned()
    {
        UpdateDropdown();
        foreach(EmoteButton button in emoteButtons){
            button.OnEmoteSelected += SendEmoteToPlayer;
        }
    }

    
    private void UpdateDropdown(){
        playersDropdown.ClearOptions();
        Debug.Log($"Runner null: {Runner == null}, activePlayers null {Runner.ActivePlayers == null}, activeplayers length: {Runner.ActivePlayers.Count()}");
        playersDropdown.AddOptions(Runner.ActivePlayers.Select(player => player.PlayerId.ToString()).ToList());
    }

    // Call this from any client to send an emote to exactly one player.
    public void SendEmoteToPlayer(EmoteType emoteType)
    {
        // Pass both the emote ID and who it's for,
        // plus the sender (InputAuthority) to know who sent it.
        RpcReceiveEmote(emoteType, Runner.ActivePlayers.ToList()[playersDropdown.value], Object.StateAuthority);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcReceiveEmote(EmoteType emoteType, PlayerRef target, PlayerRef sender)
    {
        Debug.Log("Arrived at rpcreceive");
        // Only the intended player should handle this
        if (Runner.LocalPlayer != target)
            return;

        // Display it in your UI:
        Debug.Log("at player's rpcreceive");

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