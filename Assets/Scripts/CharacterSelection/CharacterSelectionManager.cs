using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CharacterSelectionManager : NetworkBehaviour
{
    [SerializeField] private List<Material> characterMaterialList;


    private List<int> takenIndexes = new List<int>();

    private NetworkRunner networkRunner;

    void Start()
    {
        networkRunner = NetworkRunner.GetRunnerForScene(SceneManager.GetActiveScene());
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPCRequestCharacterSelect(int index, RpcInfo info = default)
    {
        if (takenIndexes.Contains(index))
        {
            Debug.LogWarning("Character already selected");
            return;//Insert UI logic of already selected
        }
        takenIndexes.Add(index);
        RPCSpawnSelected(info.Source, index);


    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPCSpawnSelected([RpcTarget] PlayerRef targetPlayer, int index)
    {
        networkRunner.SpawnAsync(characterList[Mathf.Clamp(index, 0, characterList.Count - 1)]);
    }

}