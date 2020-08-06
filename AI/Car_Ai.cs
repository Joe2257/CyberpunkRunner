using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Ai : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _carExpireTime = 5f;


    private Rigidbody _rigidBody = null;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

       StartCoroutine(DisableCar(_carExpireTime));
    }


    void Update()
    {
        MoveCar();
    }

    private void MoveCar()
    {
        _rigidBody.velocity = transform.forward * _speed;
    }

    private IEnumerator DisableCar(float Time)
    {
        yield return new WaitForSeconds(Time);

        gameObject.SetActive(false);
    }

   
}
