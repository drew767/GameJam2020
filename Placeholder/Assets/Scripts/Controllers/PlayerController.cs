using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody m_rigidbody;

    InputState m_inputState;

    [SerializeField]
    Transform m_camera;

    [SerializeField]
    Transform m_neck;

    [SerializeField]
    ColliderTrigger m_groundedTrigger;

    [SerializeField]
    float maxForwardSpeed = 1f;

    [SerializeField]
    float maxSideSpeed = 1f;

    [SerializeField]
    float horizontalSensitivity = 200f;

    [SerializeField]
    float verticalSensitivity = 200f;

    [SerializeField]
    float jumpForce = 100f;

    public bool GetIsDead()
    {
        return m_isDead;
    }
    bool m_isDead = false;

    class Timeout
    {
        float m_expirationTime;
        public Timeout(float seconds)
        {
            m_expirationTime = seconds + Time.time;
        }

        public bool isExpired()
        {
            return Time.time > m_expirationTime;
        }
    }

    Timeout jumpCooldown;


    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_inputState = GetComponent<InputState>();
    }



    // Update is called once per frame
    void Update()
    {
    }

    Vector3 Direction2Dto3D(Vector2 dir)
    {
        float yawAngle = m_neck.rotation.eulerAngles.y;
        Quaternion orientation = Quaternion.Euler(0f, yawAngle, 0f);
        
        return orientation * new Vector3(dir.x * maxSideSpeed, 0f, dir.y * maxForwardSpeed);
    }

    Quaternion CursorDeltaToRotation(Vector3 delta)
    {
        return Quaternion.Euler(new Vector3(-delta.y * verticalSensitivity, delta.x * horizontalSensitivity, 0f));
    }

    float SnapAngle180(float angle)
    {
        while (angle < -180f)
            angle += 360;
        while (angle > 180f)
            angle -= 360;
        return angle;
    }

    private void ClampCameraPitch()
    {
        Vector3 cameraAngles = m_camera.localRotation.eulerAngles;
        cameraAngles.x = Mathf.Clamp(SnapAngle180(cameraAngles.x), -70f, 70f);
        m_camera.localRotation = Quaternion.Euler(cameraAngles);
    }

    private void UpdateMovement(InputState.CharacterInputState inputState)
    {
        m_rigidbody.velocity = Direction2Dto3D(inputState.movementDirection) + new Vector3(0f, m_rigidbody.velocity.y, 0f);
    }

    private void UpdateRotation(InputState.CharacterInputState inputState)
    {
        Quaternion rotationDelta = CursorDeltaToRotation(inputState.summaryMouseDelta);
        m_camera.Rotate(m_camera.right, rotationDelta.eulerAngles.x, Space.World);
        m_neck.Rotate(Vector3.up, rotationDelta.eulerAngles.y);
        inputState.summaryMouseDelta = new Vector2();
        ClampCameraPitch();
    }

    private void FixedUpdate()
    {
        var inputState = m_inputState.GetInputState();
        UpdateMovement(inputState);
        UpdateRotation(inputState);

        if (m_groundedTrigger.IsTriggered && inputState.jump && (jumpCooldown == null || jumpCooldown.isExpired()))
        {
            m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCooldown = new Timeout(0.5f);
        }
        m_inputState.DropInputState();
    }
}
