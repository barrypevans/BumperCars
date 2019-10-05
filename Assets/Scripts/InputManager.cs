using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    private static InputManager[] InputManagers = new InputManager[4];
    public static InputManager CreateInputManager(int index)
    {
        if (null == InputManagers[index])
            InputManagers[index] = new InputManager(index);
        return InputManagers[index];
    }

    private int m_index = 0;

    public InputManager(int index)
    {
        m_index = index;
    }

    public float Horizontal
    {
        get
        {
            return Input.GetAxis("Horizontal_" + m_index.ToString());
        }
    }

    public float Vertical
    {
        get
        {
            return Input.GetAxis("Vertical_" + m_index.ToString());
        }
    }

    /*public bool ActionPressed
    {
        get
        {
            return 
        }
    }*/
}
