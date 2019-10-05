using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CarController : MonoBehaviour
{
    InputManager m_inputManager = InputManager.CreateInputManager(0);
    Vector2 velocity;

    void Update()
    {
        
    }
}
