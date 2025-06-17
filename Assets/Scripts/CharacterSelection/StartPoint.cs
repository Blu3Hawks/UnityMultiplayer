using UnityEngine;

public class StartPoint : MonoBehaviour {
    [SerializeField] private MeshRenderer meshRenderer;

    private bool isTaken;

    public bool IsTaken => isTaken;

    public void Initialize(){
        isTaken = true;
    }
}