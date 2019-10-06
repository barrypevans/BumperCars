using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{

    private int m_index = 0;
    //private PlayerInput m_playerInput;

    public InputManager(int index)
    {
        m_index = index;
    }


    public void Enable()
    {
        // m_playerInput.Enable();
    }

    public void Disable()
    {
        //m_playerInput.Disable();
    }

    public float HorizontalLeftStick
    {
        get
        {
            return Input.GetAxis("HorizontalLeft_" + m_index.ToString());
        }
    }

    public float VerticalLeftStick
    {
        get
        {
            return Input.GetAxis("VerticalLeft_" + m_index.ToString());
        }
    }
    public Vector2 LeftStick
    {
        get
        {
            return new Vector2(HorizontalLeftStick, VerticalLeftStick);
        }
    }

    public float HorizontalRightStick
    {
        get
        {
            return RightStick.x;
        }
    }
    public float VerticalRightStick
    {
        get
        {
            return RightStick.y;
        }
    }
    public Vector2 RightStick;

    public bool Accelecator
    {
        get
        {
            return Input.GetKey(KeyCode.Joystick1Button0 + 20 * m_index);
        }
    }

    public bool Submit
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Joystick1Button0 + 20 * m_index);
        }
    }

    public bool LeftBumper
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Joystick1Button4 + 20 * m_index);
        }
    }

    public bool RightBumper
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Joystick1Button5 + 20 * m_index);
        }
    }
}
