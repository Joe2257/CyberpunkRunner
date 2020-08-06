using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Joystick _joyStick    = null;

    private float _horizontalMovement;
    private float _verticalMovement;

    private bool _joyStickJump  = false;
    private bool _joystickShoot = false;

    public bool joyStickJump
    { get { return _joyStickJump; } set { _joyStickJump = value; } }

    public bool joystickShoot
    { get { return _joystickShoot; } set { _joystickShoot = value; } }

    public float horizontalMovement
    { get { return _horizontalMovement; } }


    //----------KeyInputs-----------\\

    public bool moveRight
    { get { return Input.GetKey(KeyCode.RightArrow); } }

    public bool moveLeft
    { get { return Input.GetKey(KeyCode.LeftArrow); } }

    public bool jump
    {get { return Input.GetKeyDown(KeyCode.Space); } }

    public bool shoot
    { get { return Input.GetKeyDown(KeyCode.F); } }

    public bool pause
    { get { return Input.GetKeyDown(KeyCode.Escape); } }

    //----------JoystickInputs-----------\\

    private void JoyStickMovement()
    {
        _horizontalMovement = _joyStick.Horizontal;
    }

    public void JoystickJump()
    {
       _joyStickJump = true;

    }

    public void JoystickShoot()
    {
        _joystickShoot = true;
    }

    //----------Updates-----------\\

    private void Update()
    {
        JoyStickMovement();
    }


   

}
