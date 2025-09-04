using System;
using Game_Events;
using Unity.Multiplayer.Playmode;
using UnityEngine;

public class SceneHelper : MonoBehaviour
{
    [SerializeField] private GameObject HostGameObject;
    [SerializeField] private GameObject ClientGameObject;

    [SerializeField] private Camera cam;
    private void Awake()
    {
        #if UNITY_EDITOR
            HostGameObject.SetActive(false);
            ClientGameObject.SetActive(true);
            if (CurrentPlayer.ReadOnlyTags().Length > 0 && CurrentPlayer.ReadOnlyTags()[0] == "Server")
            {
                Debug.Log("Server");
                HostGameObject.SetActive(true);
                ClientGameObject.SetActive(false);
            }
        #endif
            
            // #if UNITY_SERVER
            //         Debug.Log("Server");
            //         HostGameObject.SetActive(true);
            //         ClientGameObject.SetActive(false);
            // #endif 
        GameEvents.OnRoomStarted += HandleRoomStarted;
    }

    private void HandleRoomStarted()
    {
        
        cam.gameObject.SetActive(false);
        ClientGameObject.SetActive(false);
    }
}
