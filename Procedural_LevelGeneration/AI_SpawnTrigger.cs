using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_SpawnTrigger : MonoBehaviour
{
    private bool _spawnAI = false;

    public bool spawnAI
    { get { return _spawnAI; } }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            _spawnAI = true;
    }
}
