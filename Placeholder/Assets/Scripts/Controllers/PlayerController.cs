using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    Rigidbody m_rigidbody;
    InputState m_inputState;

    [SerializeField]
    Collider m_collider;

    [SerializeField]
    Transform m_camera;

    [SerializeField]
    Transform m_neck;

    [SerializeField]
    ColliderTrigger m_groundedTrigger;

    [Header("On the ground")]
    [SerializeField]
    float maxSpeedGround = 20f;

    [SerializeField]
    float maxForwardForceGround = 30000f;

    [SerializeField]
    float maxSideForceGround = 30000f;

    [SerializeField]
    float dragGround = 15f;

    [SerializeField]
    float jumpForceGround = 180f;

    public bool GetIsDead()
    {
        return m_isDead;
    }
    bool m_isDead = false;
    [Header("In the air")]
    [SerializeField]
    float maxSpeedAir = 5f;

    [SerializeField]
    float maxForwardForceAir = 3000f;

    [SerializeField]
    float maxSideForceAir = 3000f;

    [SerializeField]
    float jumpForceAir = 50f;

    [SerializeField]
    float dragAir = 0.1f;

    [Header("Camera Look")]
    [SerializeField]
    float horizontalSensitivity = 200f;

    [SerializeField]
    float verticalSensitivity = 200f;

    bool isGrounded { get { return m_groundedTrigger.IsTriggered; } }
    float movementForce { get { return isGrounded ? maxForwardForceGround : maxForwardForceAir; } }
    float maxSpeed { get { return isGrounded ? maxSpeedGround : maxSpeedAir; } }
    float drag { get { return isGrounded ? dragGround : dragAir; } }
    float jumpForce { get { return isGrounded ? jumpForceGround : jumpForceAir; } }
    bool useGravity { get { return !isGrounded; } }


    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_inputState = GetComponent<InputState>();
        Test();
    }

    // Update is called once per frame
    void Update()
    {
    }

    static Vector3 flatVector(Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    static Vector3 FromVector2(Vector2 v)
    {
        return new Vector3(v.x, 0f, v.y);
    }

    float GetForce(float dot, float k)
    {
        return Mathf.Clamp( 1f - 0.5f * k * (1f + dot), -1, 1);
    }

    void Test()
    {
        TestEqual(GetForce(1, 0), 1);
        TestEqual(GetForce(0, 0), 1);
        TestEqual(GetForce(-1, 0), 1);

        TestEqual(GetForce(1, 1), 0);
        TestEqual(GetForce(0, 1), 0.5f);
        TestEqual(GetForce(-1, 1), 1);
    }

    void TestEqual(float a, float b)
    {
        if (a == b)
            Debug.Log("Ok");
        else
            Debug.LogError("Fail on " + a + " " + b);
    }

    Vector3 Direction2Dto3D(Vector2 forceDirection)
    {
        Debug.Assert(forceDirection.sqrMagnitude <= 1f);
        Quaternion orientation = Quaternion.Euler(0f, m_neck.rotation.eulerAngles.y, 0f);
        Vector3 speedVector = flatVector(m_rigidbody.velocity);
        Vector3 forceVector = orientation * FromVector2(forceDirection);
        float speedScale = Mathf.Clamp01(speedVector.magnitude / maxSpeed);
        float forceScale = GetForce(Vector3.Dot(speedVector.normalized, forceVector), speedScale);
        if(forceScale < 0f)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.black, 0.1f);
        }
        Debug.DrawLine(transform.position, transform.position + speedVector, Color.green, 0.1f);
        Debug.DrawLine(transform.position, transform.position + forceVector, Color.red, 0.1f);
        return forceVector * movementForce * forceScale;
    }

    Quaternion CursorDeltaToRotation(Vector3 delta)
    {
        return Quaternion.Euler(new Vector3(-delta.y * verticalSensitivity, delta.x * horizontalSensitivity, 0f));
    }

    static float SnapAngle180(float angle)
    {
        while (angle < -180f)
            angle += 360;
        while (angle > 180f)
            angle -= 360;
        return angle;
    }

    private void UpdateMovement(InputState.CharacterInputState inputState)
    {
        m_rigidbody.AddForce(Direction2Dto3D(inputState.movementDirection));
    }

    private void UpdateRotation(InputState.CharacterInputState inputState)
    {
        Quaternion rotationDelta = CursorDeltaToRotation(inputState.summaryMouseDelta);
        if (rotationDelta.eulerAngles.sqrMagnitude != 0f)
        {
            float pitch = SnapAngle180(m_camera.localRotation.eulerAngles.x);
            pitch = SnapAngle180(pitch + rotationDelta.eulerAngles.x);
            pitch = Mathf.Clamp(pitch, -70, 70);
            m_camera.localRotation = Quaternion.Euler(new Vector3(pitch, 0f, 0f));
            m_camera.Rotate(m_camera.right, rotationDelta.eulerAngles.x, Space.World);
            m_neck.Rotate(Vector3.up, rotationDelta.eulerAngles.y);
        }
        inputState.summaryMouseDelta = new Vector2();
    }


    private void FixedUpdate()
    {
        var inputState = m_inputState.GetInputState();
        m_rigidbody.drag = drag;
        m_rigidbody.useGravity = useGravity;
        UpdateMovement(inputState);
        UpdateRotation(inputState);

        if (m_groundedTrigger.IsTriggered)
        {
            if (inputState.jump)
            {
                m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        m_inputState.DropInputState();
    }
}
