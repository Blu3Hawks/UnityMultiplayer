using UnityEngine;
using Fusion;

public class FXTrigger : MonoBehaviour
{
    [SerializeField] private GameObject fxPrefab;
    [SerializeField] private NetworkObject targetPlayer;
    [SerializeField] private float destroyAfterSeconds = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetPlayer.gameObject)
        {
            SpawnEffect(transform.position);
        }
    }

    private void SpawnEffect(Vector3 position)
    {
        if (fxPrefab != null)
        {
            GameObject instance = Instantiate(fxPrefab, position, Quaternion.identity);
            Destroy(instance, destroyAfterSeconds);
        }
    }
}
