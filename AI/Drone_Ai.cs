using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Ai : MonoBehaviour
{
    [SerializeField] private int   _healthPoints = 1;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _fireRate        = 0;
    [SerializeField] private float _maxStrafeRate   = 0;

    public ParticleSystem laser;
    public ParticleSystem explosion;

    public AudioClip explosionClip;
    public AudioClip fireClip;


    private bool  _isDead    = false;
    private bool  _canShoot  = true;
    private bool  _hasFired  = false;
    private bool  _moveLeft  = true;
    private bool  _moveRight = false;
    private float _randomStartDirection = 0;

    private AudioSource _audiosource;
    private Rigidbody   _rigidBody = null;

    void Start()
    {
        _rigidBody   = GetComponent<Rigidbody>();
        _audiosource = GetComponent<AudioSource>();

        transform.Translate(new Vector3(0, 2, 0), Space.World);


        //Randomly decide if the drone will start moving Left or Right
        _randomStartDirection = Random.Range(1, 10f);

        if (_randomStartDirection > 4)
            StartCoroutine(StrafeRight(Random.Range(.1f, _maxStrafeRate)));
        else
            StartCoroutine(StrafeLeft(Random.Range(.1f, _maxStrafeRate)));
    }


    void Update()
    {
        Shoot();
        Die();
    }

    //Move left, start moving on the opposite direction when timer expire.
    private IEnumerator StrafeLeft(float Time)
    {
        _rigidBody.velocity = transform.right * _speed;

        yield return new WaitForSeconds(Time);

        if(!_isDead)
           StartCoroutine(StrafeRight(Random.Range(1, _maxStrafeRate)));
    }

    //Move right, start moving on the opposite direction when timer expire.
    private IEnumerator StrafeRight(float Time)
    {
        _rigidBody.velocity = -transform.right * _speed;

        yield return new WaitForSeconds(Time);

        if (!_isDead)
            StartCoroutine(StrafeLeft(Random.Range(1, _maxStrafeRate)));
    }

    private void Shoot()
    {
        if (_canShoot)
        {
            _canShoot = false;

            StartCoroutine(ShootRoutine(_fireRate));
        }
    }

    private IEnumerator ShootRoutine(float Time)
    {
        laser.Play();

        if (!_hasFired)
        {
            _audiosource.PlayOneShot(fireClip);
            _hasFired = true;
        }
       

        yield return new WaitForSeconds(Random.Range(Time, Time + 2));

        if(!_isDead)
        _canShoot = true;
        _hasFired = false;
    }

    public void TakeDamage(int amount)
    {
        _healthPoints -= amount;
    }

    private void Die()
    {
        if (_healthPoints == 0 && !_isDead)
        {
            explosion.Play();
            _audiosource.PlayOneShot(explosionClip);

            _isDead = true;
            StopAllCoroutines();

            _rigidBody.useGravity = true;
            _canShoot = false;
        }
    }
}
