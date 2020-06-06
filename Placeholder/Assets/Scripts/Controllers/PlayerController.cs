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

    [Header("Other")]
    [SerializeField]
    float jumpCooldown = 0.3f;

    Timeout m_jumpTimeout;

    public bool GetIsDead()
    {
        return m_isDead;
    }
    bool m_isDead = false;

    bool isGrounded { get { return m_groundedTrigger.IsTriggered; } }
    float movementForce { get { return isGrounded ? maxForwardForceGround : maxForwardForceAir; } }
    float maxSpeed { get { return isGrounded ? maxSpeedGround : maxSpeedAir; } }
    float drag { get { return isGrounded ? dragGround : dragAir; } }
    float jumpForce { get { return TestJumpDistance() ? jumpForceGround : jumpForceAir; } }

    bool? jumpTestResultCache;
    bool TestJumpDistance()
    {
        if (jumpTestResultCache.HasValue)
            return jumpTestResultCache.Value;
        Ray downProbe = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        float probeDistance = 3f;
        jumpTestResultCache = false;
        if (Physics.SphereCast(downProbe, 0.5f, out hit, probeDistance, PhysicsLayers.MaskDefault))
        {
            jumpTestResultCache = true;
        }
        return jumpTestResultCache.Value;
    }

    bool CanJump()
    {
        return m_jumpTimeout.ExpiredOrNull() && TestJumpDistance();
    }

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



    float GetForceScaler(float dot, float k)
    {
        return Mathf.Clamp( 1f - k * (1f + dot) / 2f, -1, 1);
    }

    Vector3 Direction2Dto3D(Vector2 forceDirection)
    {
        Quaternion orientation = Quaternion.Euler(0f, m_neck.rotation.eulerAngles.y, 0f);
        Vector3 speedVector = MathHelper.FlatVector(m_rigidbody.velocity);
        Vector3 forceVector = orientation * MathHelper.FromVector2(forceDirection);
        float speedScale = Mathf.Clamp01(speedVector.magnitude / maxSpeed);
        float forceScale = GetForceScaler(Vector3.Dot(speedVector.normalized, forceVector), speedScale);
        return forceVector * movementForce * forceScale;
    }

    Quaternion CursorDeltaToRotation(Vector3 delta)
    {
        return Quaternion.Euler(new Vector3(-delta.y * verticalSensitivity, delta.x * horizontalSensitivity, 0f));
    }

    private void UpdateMovement(InputState.CharacterInputState inputState)
    {
        m_rigidbody.AddForce(Direction2Dto3D(inputState.movementDirection));
    }

    float GetNewPitch(Quaternion rotationDelta)
    {
        float pitch = MathHelper.SnapAngle180(m_camera.localRotation.eulerAngles.x);
        pitch = MathHelper.SnapAngle180(pitch + rotationDelta.eulerAngles.x);
        return Mathf.Clamp(pitch, -70, 70);
    }

    private void UpdateRotation(InputState.CharacterInputState inputState)
    {
        Quaternion rotationDelta = CursorDeltaToRotation(inputState.summaryMouseDelta);
        if (rotationDelta.eulerAngles.sqrMagnitude != 0f)
        {
            m_camera.localRotation = Quaternion.Euler(new Vector3(GetNewPitch(rotationDelta), 0f, 0f));
            m_camera.Rotate(m_camera.right, rotationDelta.eulerAngles.x, Space.World);
            m_neck.Rotate(Vector3.up, rotationDelta.eulerAngles.y);
        }
        inputState.summaryMouseDelta = new Vector2();
    }


    private void FixedUpdate()
    {
        var inputState = m_inputState.GetInputState();
        m_rigidbody.drag = drag;
        UpdateMovement(inputState);
        UpdateRotation(inputState);

        if (inputState.jump && CanJump())
        {
            m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            m_rigidbody.AddForce(new Vector3(0f, -m_rigidbody.velocity.y, 0f) * m_rigidbody.mass, ForceMode.Impulse);
            m_rigidbody.drag = dragAir;
            m_jumpTimeout = new Timeout(jumpCooldown);
        }

        jumpTestResultCache = null;
    }
}
