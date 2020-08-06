using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Null, Running, Dead }
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _sideWallDetectionDistance  = 3;
    [SerializeField] private float _rotationSpeed              = 20f;
    [SerializeField] private float _gravityForce               = 10f;


    //Joystick axis tresholds\\
    private float horizontalMovementTresholdRight =  0.2f;
    private float horizontalMovementTresholdLeft  = -0.2f;

    private bool _wallDetected    = false;
    private bool _canJump         = false;
    private bool _canMove         = true;

    [Header("AnimationParameters")]
    public string stumbleParameter = "";
    private int _stumbleHash;


    private Vector3             _moveDirection;

    private PlayerSystem        _playerSystem;
    private PlayerInput         _playerInput;

    private Animator            _animator;
    private AudioSource         _audioSource;
    private CharacterController _charController;
    
    void Start()
    {
        _animator         = GetComponent<Animator>();
        _charController   = GetComponent<CharacterController>();
        _audioSource      = GetComponent<AudioSource>();

        _playerSystem     = GetComponent<PlayerSystem>();
        _playerInput      = GetComponent<PlayerInput>();


        InitializePlayer();
    }

    private void InitializePlayer()
    {
        _playerSystem.currentSpeed = _playerSystem.speed;

        _stumbleHash = Animator.StringToHash(stumbleParameter);
    }

    
    public void Update()
    {
        MovementUpdate();
        PlayerStateUpdater();
        PlayerAnimatorUpdater(_playerSystem.playerState);
    }

    //-----------Updates----------\\

    //Update the PlayerState based on the current hp level;
    private void PlayerStateUpdater()
    {
        if (_playerSystem.healthPoints != 0)
            _playerSystem.playerState = PlayerState.Running;
        else
        {
            _playerSystem.playerState = PlayerState.Dead;
            _canMove = false;

            _charController.enabled = false;
        }
    }


    //Update the animations based on the PlayerState.
    private void PlayerAnimatorUpdater(PlayerState playerState)
    {
        switch (playerState)
        {
            default:
                _animator.SetFloat("Speed", 0);
                break;

            case PlayerState.Running:
                _animator.SetFloat("Speed", 1);

                if (_playerSystem.powerUp)
                { _animator.SetFloat("SpeedMultiplier", 1.6f); }
                else
                { _animator.SetFloat("SpeedMultiplier", 1.2f); }

                if (_canJump)
                    _animator.SetTrigger("Jump");
                break;

            case PlayerState.Dead:
                _animator.SetTrigger("Dead");
                break;
        }

        if (_playerSystem.stumble)
        {
            _animator.SetTrigger("Stumble");

            _playerSystem.stumble = false;
        }
    }


    //----------Movement-----------\\

    private void MovementUpdate()
    {
        SpeedCalculation();
        MovePlayer();
    }

    //Equalize Speed to avoid diagonal speed increase;
    private void SpeedCalculation()
    {
        if (_playerInput.moveRight || _playerInput.moveLeft || _playerInput.horizontalMovement > horizontalMovementTresholdRight || _playerInput.horizontalMovement < horizontalMovementTresholdLeft)
        {
            _playerSystem.currentSpeed = _playerSystem.speed - (_playerSystem.strafeSpeed /2);
        }
        else
        {
            _playerSystem.currentSpeed = _playerSystem.speed;
        }

    }


    //Constatly move forward, apply gravity, jump on input;
    private void MovePlayer()
    {
        if (_charController.isGrounded)
        {
            Strafe();
            _moveDirection = new Vector3(0, 0, 1);
            _moveDirection = transform.TransformDirection(_moveDirection).normalized;
            _moveDirection *= _playerSystem.currentSpeed;
        }

        if (_playerInput.jump && _charController.isGrounded || _playerInput.joyStickJump && _charController.isGrounded)
        {
            _audioSource.PlayOneShot(_playerSystem.clips[0]);

            _playerInput.joyStickJump = false;
            _canJump = true;
            _moveDirection.y = _playerSystem.jumpSpeed;
        }
        else
            _canJump = false;

        if (_animator.GetFloat("StumbleParameter") < 1 && _canMove)
        {
            _moveDirection.y -= _gravityForce * Time.deltaTime;
            _charController.Move(_moveDirection * Time.deltaTime);
        }
    }


    // Rotate right or left if no boundary is detected in the direction of the movement;
    //Use _sideWallDetectionDistance to increase or decrease wall distance detection;
    private void Strafe()
    {
        if (_playerInput.moveRight || _playerInput.horizontalMovement > horizontalMovementTresholdRight)
        {
            Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.right));

            if (Physics.Raycast(ray, _sideWallDetectionDistance))
            { _wallDetected = true;  return; }
            else
            {
                _wallDetected = false;

                transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        if (_playerInput.moveLeft || _playerInput.horizontalMovement < horizontalMovementTresholdLeft)
        {
            Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.left));

            if (Physics.Raycast(ray, _sideWallDetectionDistance))
            { _wallDetected = true; return; }
            else
            {
                _wallDetected = false;

                transform.Rotate(Vector3.up * -_rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}
