using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CarController : MonoBehaviour
{

    private static readonly float kMaxVelocity = 20;
    private static readonly float kAcceleration = 2.5f;
    private static readonly float kMaxTurnSpeed = 50;
    private static readonly float kTurnAcceleration = 5;
    private static readonly float kDeceleration = 30f;

    private CameraController m_cameraController;

    private bool m_dead;
    private Coroutine m_bumpCo = null;
    private bool m_isBumping = false;
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
            m_controlAmount = Mathf.Min(m_controlAmount, 1);
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
        }
    }

    public void Bump(Vector3 direction)
    {
        if(m_bumpCo != null)
        {
            StopCoroutine(m_bumpCo);
        }

        m_isBumping = true;
        m_bumpCo = StartCoroutine(Co_Bump());
        Velocity = direction.normalized * 20;
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
        m_cameraController = m_camera.transform.parent.GetComponent<CameraController>();
    }

    private void OnEnable()
    {
        if(null == m_inputManager)
            m_inputManager = new InputManager(m_playerIndex);
        m_inputManager.Enable();
    }

    private void Update()
    {
        if (m_dead) return;

        Movement();
        if (!m_isBumping)
        {
            UpdateVisuals();
            Tilt();
        }
        CheckForDeath();

    }

    private void OnDisable()
    {
        m_inputManager.Disable();
    }

    private void Movement()
    {
        if (!m_isBumping)
        {
            if (m_inputManager.Accelecator)
            {
                ControlAmount = 1;
            }
            else
            {
                Velocity -= Velocity.normalized * kDeceleration * Time.deltaTime;
                ControlAmount -= 2.0f * Time.deltaTime;
            }
        }

        m_stickLag = Vector2.Lerp(m_stickLag, m_inputManager.LeftStick, 2.7f * Time.deltaTime);
        m_heading = Mathf.Atan2(m_stickLag.x, m_stickLag.y) * Mathf.Rad2Deg;

        Velocity += (Quaternion.Euler(0, m_heading, 0) * GetMajorCameraAxis()) * kAcceleration * ControlAmount;
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

    private IEnumerator Co_Bump()
    {
        m_cameraController.AddShake(1);
        ControlAmount = 0;
        for (int i=0; i<5; ++i)
        {
            ControlAmount += .1f;
            yield return new WaitForSeconds(.25f);
        }
        m_isBumping = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_dead) return;
        if (collision.collider.tag == "Car")
        {
            Bump(Vector3.Normalize(transform.position - collision.collider.transform.position));
            LevelManger.Instance.TakeOutTile(collision.contacts[0].point);
        }
    }

    private void CheckForDeath()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position+ Vector3.up, Vector3.down,100);
        bool groundHits = hits.Any(hit => hit.collider.tag == "GroundTile");
        if(!groundHits && !m_dead && !m_isBumping)
        {
            m_dead = true;
            StartCoroutine(Co_DeathAnim());
        }
    }

    private IEnumerator Co_DeathAnim()
    {
        m_rigidbody.isKinematic = true;
        yield return new WaitForSeconds(.5f);
        var counter = 0;
        while (counter < 10)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(.5f, 2.0f, .5f), .3f);
            counter++;
            yield return new WaitForSeconds(.01f);
        }
        counter = 0;
        while (counter < 10)
        {
            transform.transform.position += Vector3.down*8;
            counter++;
            yield return new WaitForSeconds(.01f);
        }
    }

    [SerializeField]
    private Camera m_camera;
    [SerializeField]
    private Transform m_top;
    private Rigidbody m_rigidbody;
    private float m_turnSpeed = 0;
    private float m_heading = 0;
    private float m_controlAmount = 0;
}
