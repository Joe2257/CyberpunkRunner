using System.Collections;
using UnityEngine;

public class RangedSoldier_Ai : Main_Ai
{
    [SerializeField] private int   _healthPoints = 1;
    [SerializeField] private float _fireRate = 0;

    public ParticleSystem  laser;
    [Space]
    public AudioClip       fireClip;
    public AudioClip       deathClip;

    private bool _canShoot = true;
    private bool _fire     = false;
    private bool _hasFired = false;
    private bool _isDead   = false;

    public string _shootParameter = "";
    private int   _shootHash = -1;

    private AudioSource _audiosource;
    private Animator    _animator;
    private Rigidbody   _rigidBody = null;

    void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        _rigidBody   = GetComponent<Rigidbody>();
        _animator    = GetComponent<Animator>();

        _shootHash = Animator.StringToHash(_shootParameter);
    }

    void Update()
    {
        Shoot();
        Death();
    }

    //Start the shooting animation and shoot every time the animation is in the correct position.
    private void Shoot()
    {
        if (_canShoot && !_isDead)
        {
            _canShoot = false;
            _animator.SetTrigger("Shoot");

            StartCoroutine(ShootRoutine(_fireRate));
        }

        if (_animator.GetFloat("ShootParameter") > 0 && !_fire)
        {
            _fire = true;
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

        yield return new WaitForSeconds(Random.Range(Time, Time+2));

        _fire = false;
        _hasFired = false;
        _canShoot = true;
    }

    public void TakeDamage(int amount)
    {
        _healthPoints -= amount;
    }

    private void Death()
    {
        if (_healthPoints == 0 && !_isDead)
        {
            _audiosource.PlayOneShot(deathClip);
            _isDead = true;
            _animator.SetTrigger("Die");
        }
    }
}
