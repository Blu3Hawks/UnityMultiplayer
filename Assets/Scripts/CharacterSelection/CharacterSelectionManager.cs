using System.Collections.Generic;
using Fusion;
using UnityEngine;
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

    private NetworkRunner networkRunner;

    [Networked] private int selectedIndex {get; set;}

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


    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPCRequestCharacterSelect(int index, RpcInfo info = default)
    {
        if (takenIndexes.Contains(index))
        {
            RPCCharacterAlreadySelected(info.Source);
            return;//Insert UI logic of already selected
        }
        takenIndexes.Add(index);
        RPCSpawnSelected(info.Source, index);


    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private async void RPCSpawnSelected([RpcTarget] PlayerRef targetPlayer, int index)
    {
        startingPoints[index].Initialize();
        NetworkObject spawnedObject = await networkRunner.SpawnAsync(characterList[index].gameObject, startingPoints[index].transform.position + Vector3.up, Quaternion.identity);
        selectedIndex = index;

    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCCharacterAlreadySelected([RpcTarget] PlayerRef targetPlayer)
    {
        if(networkRunner.LocalPlayer == targetPlayer){
            Debug.LogWarning("Character already selected");

            LayoutParent.gameObject.SetActive(true);

        }

    }


    

}