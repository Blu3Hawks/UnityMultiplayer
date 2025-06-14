using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class CharacterButton : MonoBehaviour {
    private int currentIndex;

    [SerializeField] private TextMeshProUGUI buttonText;

    public event UnityAction<int> OnColorSelected;
    public void InitializeButton(int index, string materialName)
    {
        currentIndex = index;
        buttonText.SetText(materialName);
    }

    public void HandleColorSelection()
    {
        OnColorSelected?.Invoke(currentIndex);
    }
}