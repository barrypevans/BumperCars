using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{

    private int m_index = 0;
    private PlayerInput m_playerInput;

    public InputManager(int index)
    {
        m_index = index;
        m_playerInput = new PlayerInput();
        m_playerInput.Enable();
        m_playerInput.Movement.LeftStick.performed += context => LeftStick = context.ReadValue<Vector2>();
        m_playerInput.Movement.RightStick.performed += context => RightStick = context.ReadValue<Vector2>();
        m_playerInput.Movement.Accelerator.performed += context => Accelecator = context.ReadValue<float>() > .05f;
        m_playerInput.Movement.Accelerator.canceled += context => Accelecator = context.ReadValue<float>() > .05f;
    }


    public void Enable()
    {
        m_playerInput.Enable();
    }

    public void Disable()
    {
        m_playerInput.Disable();
    }

    public float HorizontalLeft
    {
        get
        {
            return LeftStick.x;
        }
    }

    public float VerticalLeft
    {
        get
        {
            return LeftStick.y;
        }
    }
    public Vector2 LeftStick;

    public float HorizontalRight
    {
        get
        {
            return RightStick.x;
        }
    }
    public float VerticalRight
    {
        get
        {
            return RightStick.y;
        }
    }
    public Vector2 RightStick;

    public bool Accelecator;
}
