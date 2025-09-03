using System;
using UnityEngine;

public class SceneHelper : MonoBehaviour
{
    [SerializeField] private GameObject HostGameObject;
    [SerializeField] private GameObject ClientGameObject;
    
    private void Awake()
    {
        #if UNITY_EDITOR
            HostGameObject.SetActive(false);
            ClientGameObject.SetActive(true);
        #endif
    }
}
