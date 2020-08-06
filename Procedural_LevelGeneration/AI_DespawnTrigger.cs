using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_DespawnTrigger : MonoBehaviour
{
    private bool _despawn = false;

    public bool despawn
    { get { return _despawn; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _despawn = true;
        }
    }
}
