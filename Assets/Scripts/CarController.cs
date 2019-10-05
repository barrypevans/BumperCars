using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CarController : MonoBehaviour
{

    private static readonly float kMaxVelocity = 20;
    private static readonly float kAcceleration = 50f;
    private static readonly float kMaxTurnSpeed = 3;
    private static readonly float kTurnAcceleration = 6f;
    private static readonly float kDeceleration = 30f;

    [SerializeField]
    private int m_playerIndex = 0;
    private Vector2 m_stickLag;

    private float ControlAmount
    {
        get
        {
            return m_controlAmount;
        }
        set
        {
            m_controlAmount = Mathf.Max(value, 0);
        }
    }

    private float TurnSpeed
    {
        get
        {
            return m_turnSpeed;
        }
        set
        {
            m_turnSpeed = Mathf.Min(Mathf.Abs(value), kMaxTurnSpeed) * Mathf.Sign(value);
            //m_turnSpeed = Mathf.Max(value, 0) ;
        }
    }

    InputManager m_inputManager;
    private Vector3 Velocity
    {
        get
        {
            return m_rigidbody.velocity;
        }
        set
        {
            m_rigidbody.velocity = Mathf.Min(value.magnitude, kMaxVelocity) * value.normalized;// * Mathf.Sign(value.magnitude);
        }
    }

    private Transform CameraTransform
    {
        get
        {
            return m_camera.transform;
        }
    }

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if(null == m_inputManager)
            m_inputManager = new InputManager(m_playerIndex);
        m_inputManager.Enable();
    }

    private void Update()
    {
        Movement();
        UpdateVisuals();
        Tilt();
        
    }

    private void OnDisable()
    {
        m_inputManager.Disable();
    }

    private void Movement()
    {
        if (m_inputManager.Accelecator)
        {
            ControlAmount = 1;
        }
        else
        {
            Velocity -= Velocity.normalized * kDeceleration * Time.deltaTime;
        }

        m_stickLag = Vector2.Lerp(m_stickLag, m_inputManager.LeftStick ,.65f * Time.deltaTime);
        m_heading =  Mathf.Atan2(m_stickLag.x, m_stickLag.y) * Mathf.Rad2Deg;
        
        Velocity += (Quaternion.Euler(0, m_heading, 0) * GetMajorCameraAxis()) * kAcceleration* ControlAmount;

        ControlAmount -= 2.0f * Time.deltaTime;
    }

    private void UpdateVisuals()
    {
        if (Velocity.magnitude > 2f)
        {
            Vector3 targetDir = Vector3.Lerp(transform.forward, Velocity.normalized, Time.deltaTime * 100);
            transform.localRotation =  Quaternion.LookRotation(targetDir, Vector3.up);
        }
    }

    private Vector3 GetStickAcceration()
    {
        m_heading =  Mathf.Atan2(m_inputManager.VerticalLeft, m_inputManager.HorizontalLeft);
        return GetMajorCameraAxis() * kAcceleration  * Time.deltaTime;
    }

    private Vector3 GetMajorCameraAxis()
    {
        Vector3 projectedPoint = ProjectPointOntoPlane(CameraTransform.forward, Vector3.zero, Vector3.up);
        return Vector3.Normalize(projectedPoint);
    }

    private Vector3 ProjectPointOntoPlane(Vector3 point, Vector3 planePoint, Vector3 normal)
    {
        Vector3 v = point - planePoint;
        float dist = Vector3.Dot(v, normal);
        return point - dist * normal;
    }

    private void Tilt()
    {
        m_top.localRotation = Quaternion.Slerp(m_top.localRotation, Quaternion.Euler(0, 0, m_inputManager.HorizontalLeft * 8), Time.deltaTime*10);
    }

    [SerializeField]
    private Camera m_camera;
    [SerializeField]
    private Transform m_top;
    private Rigidbody m_rigidbody;
    private float m_turnSpeed = 0;
    private float m_heading = 0;
    private float m_controlAmount = 1;
}
