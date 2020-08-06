using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSystem : MonoBehaviour
{
    [Header("Player")]
    public PlayerState playerState;
    public LayerMask   layerMask;

    [SerializeField] private float _speed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _strafeSpeed;
    [SerializeField] private int   _healthPoints;

    [Header("Projectile")]
    public ParticleSystem _laser;

    public float speed
    { get { return _speed; } set { _speed = value; } }
    public float strafeSpeed
    { get { return _strafeSpeed; } set { _strafeSpeed = value; } }
    public float currentSpeed
    { get { return _currentSpeed; } set { _currentSpeed = value; } }
    public float jumpSpeed
    { get { return _jumpSpeed; } set { _jumpSpeed = value; } }
    public float healthPoints
    { get { return _healthPoints; } }
    public bool powerUp
    { get { return _powerUp; }}
    public bool stumble
    { get { return _stumble; } set { _stumble = value; } }

    [Header("PlayerUI")]
    public GameObject   uiObjects;
    public GameObject   pauseMenu;
    public GameObject   deathScreen;
    public Joystick     joyStick;
    public Button       jumpButton;
    public Button       shootButton;
    public GameObject[] healthIcons;
    public Text         coinCounter;

    [Header("Audio (Hoover on array for tooltip)")]
    [Tooltip(" 0 = Jump\n 1 = Shoot\n 2 = TakeDamage\n 3 = Stumble")]
    public AudioClip[] clips;
    [Tooltip(" 0 = Coins\n 1 = Health\n 2 = Ammo\n 3 = PowerUp")]
    public AudioClip[] collectablesClips;

    private float _currentSpeed;
    private float _powerUpMaxTime      = 15f;
    private int   _coinsCollected      = 0;
    private int   _currentAmmo         = 200;
    private bool  _isPaused            = false;
    private bool  _pauseButtonPressed  = false;
    private bool  _powerUp             = false;
    private bool  _stumble             = false;


    private PlayerInput        _playerInput;
    private CollectableItem    _objectCollected;

    private AudioSource _audioSource;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _audioSource = GetComponent<AudioSource>();

        InitializePlayer();
    }

    private void InitializePlayer()
    {
        if (PlayerPrefs.GetInt("Mobile") == 1)
        {
            joyStick.gameObject.SetActive(true);
            jumpButton.gameObject.SetActive(true);
            shootButton.gameObject.SetActive(true);
        }   
        else
        {
            joyStick.gameObject.SetActive(false);
            jumpButton.gameObject.SetActive(false);
            shootButton.gameObject.SetActive(false);
        }

        deathScreen.SetActive(false);
    }

    //------------Updates------------\\

    private void Update()
    {
        UIUpdates();
        Shoot();

        PauseGame();

        PlayerUpdates();
    }

    private void UIUpdates()
    {
        coinCounter.text = "Coins = " + _coinsCollected;
    }

    private void PlayerUpdates()
    {
        FrontWallDetection();

        if (playerState == PlayerState.Dead)
            deathScreen.SetActive(true);
    }


    //------------Mechanics------------\\

    private void Shoot()
    {
        if (_currentAmmo != 0 && _playerInput.shoot || _currentAmmo != 0 && _playerInput.joystickShoot)
        {
            _audioSource.PlayOneShot(clips[1]);

            _playerInput.joystickShoot = false;

            _laser.Play();

            _currentAmmo--;
        }

    }

    //Constantly check for a wall in front of the player,
    //end the game if the player smash into a wall.
    private void FrontWallDetection()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f, layerMask))
        {
            if (hit.collider.CompareTag("WorldCollider"))
            { _healthPoints = 0;}
        }
    }

    private IEnumerator PowerUp(float Time)
    {
        _speed += 10f;
        _strafeSpeed += 7f;
        _powerUp = true;

        yield return new WaitForSeconds(Time);

        _speed -= 10f;
        _strafeSpeed -= 7f;
        _powerUp = false;
    }

    public void TakeDamage(int amount)
    {
        _audioSource.PlayOneShot(clips[2]);

        _healthPoints -= amount;

        if(_healthPoints > 0)
        healthIcons[_healthPoints + 1].SetActive(false);
    }

    //------------Player_UI------------\\

    public void OnPauseButtonClick()
    {
        _pauseButtonPressed = true;
    }

    private void PauseGame()
    {
        if (_playerInput.pause && !_isPaused || _pauseButtonPressed && !_isPaused)
        {
            _isPaused = true;

            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            uiObjects.SetActive(false);
        }
        else if (_playerInput.pause && _isPaused || !_pauseButtonPressed && _isPaused)
        {
            _isPaused           = false;
            _pauseButtonPressed = false;

            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            uiObjects.SetActive(true);
        }
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene("City");
    }

    public void OnReturnButtonClick()
    {
        _isPaused           = false;
        _pauseButtonPressed = false;

        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        uiObjects.SetActive(true);
    }

    public void OnQuitToMenuButtonClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    //------------Triggers------------\\

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactive"))
        {
            _objectCollected = other.GetComponent<CollectableItem>();

            if (_objectCollected.itemType == ItemType.Coin)
            {
                _coinsCollected++;

                _audioSource.PlayOneShot(collectablesClips[0]);
            }
            else if (_objectCollected.itemType == ItemType.MedPack)
            {
                if (_healthPoints != 3)
                {
                    _healthPoints++;

                    _audioSource.PlayOneShot(collectablesClips[1]);

                    healthIcons[_healthPoints].SetActive(true);
                }
            }
            else if (_objectCollected.itemType == ItemType.Ammo)
            {
                _currentAmmo += 5;

                _audioSource.PlayOneShot(collectablesClips[2]);
            }
            else if (_objectCollected.itemType == ItemType.PowerUp)
            {
                StartCoroutine(PowerUp(_powerUpMaxTime));

                _audioSource.PlayOneShot(collectablesClips[3]);
            }

            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            _audioSource.PlayOneShot(clips[3]);

            if (_healthPoints != 0)
            {
                healthIcons[_healthPoints].SetActive(false);
                _healthPoints--;
            }

            if (_healthPoints > 0)
                _stumble = true;
        }
        else if (other.gameObject.CompareTag("Barrier"))
        {

            _healthPoints = 0;

        }
    }
}
