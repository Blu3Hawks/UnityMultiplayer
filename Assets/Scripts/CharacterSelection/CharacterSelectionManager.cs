using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class CharacterSelectionManager : NetworkBehaviour
{
    //Did not make 10 different prefabs since they're all the same anyways right now
    //So just made 10 different colors(materials) for the same prefab
    [SerializeField] private List<PlayerManager> characterList;

    [SerializeField] private List<StartPoint> startingPoints;

    [SerializeField] private CharacterButton characterButton;

    [SerializeField] private RectTransform LayoutParent;

    private List<int> takenIndexes = new List<int>();
    
    private List<PlayerManager> playerManagers = new List<PlayerManager>();
    
    public List<PlayerManager> PlayerManagers => playerManagers;

    private NetworkRunner networkRunner;

    [Networked] private int selectedIndex {get; set;}

    public event UnityAction OnAllPlayersSelected;

    void Start()
    {
        networkRunner = NetworkRunner.GetRunnerForScene(SceneManager.GetActiveScene());
        for(int i = 0; i< characterList.Count; i++) {
            CharacterButton current = Instantiate(characterButton, LayoutParent);
            current.InitializeButton(i, $"{characterList[i].name}");
            current.OnColorSelected += HandleCharacterSelected;
        }
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    private void HandleCharacterSelected(int index) {
        RPCRequestCharacterSelect(index);
        LayoutParent.gameObject.SetActive(false);
        
        
    }


    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private async void RPCRequestCharacterSelect(int index, RpcInfo info = default)
    {
        if (takenIndexes.Contains(index))
        {
            RPCCharacterAlreadySelected(info.Source);
            return;//Insert UI logic of already selected
        }
        takenIndexes.Add(index);
        startingPoints[index].Initialize();
        Vector3 pos = startingPoints[index].transform.position;
        NetworkObject spawnedObject = await networkRunner.SpawnAsync(characterList[index].gameObject, pos + Vector3.up, Quaternion.identity, info.Source);
        PlayerManager current = spawnedObject.GetComponent<PlayerManager>();
        current.TeleportToPos(pos);
        
        current.ToggleControls(false);
        if(current && !playerManagers.Contains(current)) playerManagers.Add(current);
        selectedIndex = index;

        if (playerManagers.Count == networkRunner.ActivePlayers.Count())
        {
            AllPlayersSelected();
        }//TODO: check if server is player
        

    }

    private void AllPlayersSelected()
    {
        
        OnAllPlayersSelected?.Invoke();
    }

    


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCCharacterAlreadySelected([RpcTarget] PlayerRef targetPlayer)
    {
        Debug.LogWarning("Character already selected");

        LayoutParent.gameObject.SetActive(true);
    }


    

}