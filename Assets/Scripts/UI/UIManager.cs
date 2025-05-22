using Fusion;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerJoinedText;

    public void PlayerConnected(PlayerRef player)
    {
        playerJoinedText.SetText($"Player: {player.PlayerId} Joined the lobby");
    }
}
