using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class EmoteUiManager : MonoBehaviour {
        [Header("Emotes")]
        [SerializeField] private Sprite[] emoteSprites;
        [SerializeField] private Image emoteImage;
        
        public void ShowEmote(EmoteType emoteType, PlayerRef sender) {
            Debug.Log("at player's showemote");

            emoteImage.sprite = emoteSprites[(int)emoteType];
            emoteImage.gameObject.SetActive(true);
        }
    }
}