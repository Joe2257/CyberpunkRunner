using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private bool _isAiProjectile = false;
    [SerializeField] private int _damage = 1;

    //Determine what have been hit by the particle collider.
    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Player") && _isAiProjectile)
        {
            other.GetComponent<PlayerSystem>().TakeDamage(_damage);
        }

        if (other.gameObject.CompareTag("AI") && !_isAiProjectile)
        {
            if (other.gameObject.GetComponent<Drone_Ai>())
            { other.gameObject.GetComponent<Drone_Ai>().TakeDamage(_damage); }
            else if (other.gameObject.GetComponent<MeleeSoldier_Ai>())
            { other.gameObject.GetComponent<MeleeSoldier_Ai>().TakeDamage(_damage); }
            else if (other.gameObject.GetComponent<RangedSoldier_Ai>())
            { other.gameObject.GetComponent<RangedSoldier_Ai>().TakeDamage(_damage); }
        }
    }
}
