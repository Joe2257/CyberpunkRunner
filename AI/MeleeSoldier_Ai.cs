using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSoldier_Ai : Main_Ai
{
    [SerializeField] private float _speed         = 5f;
    [SerializeField] private int   _healthPoints  = 1;

    private bool _isDead = false;

    public BoxCollider  _shieldCollider;

    private Animator     _animator;
    private Rigidbody    _rigidBody;

    void Start()
    {
        _animator     = GetComponent<Animator>();
        _rigidBody    = GetComponent<Rigidbody>();
    }


    void Update()
    {
        MoveSoldier();
        Death();
    }


    private void MoveSoldier()
    {
        if (!_isDead)
        {
            _rigidBody.velocity = transform.forward * _speed;
            _animator.SetFloat("Speed", 1);
        }
    }

    public void TakeDamage(int amount)
    {
        _healthPoints -= amount;
    }

    private void Death()
    {
        if (_healthPoints == 0)
        {
            _isDead = true;

            _animator.SetFloat("Speed", 0);
            _animator.SetTrigger("Die");

            _shieldCollider.enabled = false;
        }
        
    }
}
