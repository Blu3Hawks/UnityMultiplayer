using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EmoteButton : MonoBehaviour {
    [SerializeField] private EmoteType emoteType;

    public event UnityAction<EmoteType> OnEmoteSelected;
    public void SendEmote(){
        OnEmoteSelected?.Invoke(emoteType);
    }
}