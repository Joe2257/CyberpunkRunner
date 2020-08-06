using UnityEngine;

public class PlatformSpawnTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool spawnNewPlatforms = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            spawnNewPlatforms = true;
        }
    }
}
