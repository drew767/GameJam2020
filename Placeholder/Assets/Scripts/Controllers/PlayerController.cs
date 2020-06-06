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
    ColliderTrigger m_groundedTrigger;

    [Header("On the ground")]
    [SerializeField]
    float maxSpeedGround = 20f;

    [SerializeField]
    float movementForceGround = 30000f;

    [SerializeField]
    float dragGround = 15f;

    [Header("In the air")]
    [SerializeField]
    float maxSpeedAir = 5f;

    [SerializeField]
    float movementForceAir = 3000f;

    [SerializeField]
    float dragAir = 0.1f;

    [Header("Camera Look")]
    [SerializeField]
    float horizontalSensitivity = 200f;

    [SerializeField]
    float verticalSensitivity = 200f;

    [Header("Jumps")]
    [SerializeField]
    float jumpCooldown = 0.3f;

    [SerializeField]
    int jumpCharges = 2;

    [SerializeField]
    float jumpForce = 600f;

    [Header("Dashes")]
    [SerializeField]
    float dashCooldown = 0.3f;

    [SerializeField]
    float dashDuration = 0.1f;

    [SerializeField]
    float maxSpeedDash = 60f;

    [SerializeField]
    int dashCharges = 2;

    [Header("Other")]
    [SerializeField]
    int health = 100;

    Timeout m_jumpTimeout;
    Timeout m_dashTimeout;
    Timeout m_dashContinue;

    int m_jumpCharges = 2;
    int m_dashCharges = 2;

    public bool GetIsDead()
    {
        return health > 0;
    }

    public void OnTakeDamage(int damage)
    {
        health -= damage;
    }

    public bool isGrounded { get { return m_groundedTrigger.IsTriggered; } }
    public float movementForce { get { return isGrounded ? movementForceGround : movementForceAir; } }
    public float maxSpeed { get { return isGrounded ? maxSpeedGround : maxSpeedAir; } }
    public float drag { get { return (isGrounded && !isDashing) ? dragGround : dragAir; } }
    public bool isDashing { get { return !m_dashContinue.ExpiredOrNull(); } }
    public float speed { get { return m_rigidbody.velocity.magnitude; } }

    bool? jumpTestResultCache;
    bool TestJumpDistance()
    {
        if (jumpTestResultCache.HasValue)
            return jumpTestResultCache.Value;
        Ray downProbe = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        float probeDistance = 2f;
        jumpTestResultCache = false;
        if (Physics.SphereCast(downProbe, 0.5f, out hit, probeDistance, PhysicsLayers.MaskDefault))
        {
            jumpTestResultCache = true;
        }
        return jumpTestResultCache.Value;
    }
        
    bool CanJump()
    {
        return m_jumpTimeout.ExpiredOrNull() && m_jumpCharges > 0;
    }

    bool CanDash()
    {
        return m_dashTimeout.ExpiredOrNull() && m_dashCharges > 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_inputState = GetComponent<InputState>();
    }

    Vector3 Direction2Dto3D(Vector2 forceDirection)
    {
        Quaternion orientation = Quaternion.Euler(0f, m_camera.rotation.eulerAngles.y, 0f);
        Vector3 speedVector = MathHelper.FlatVector(m_rigidbody.velocity);
        Vector3 forceVector = orientation * MathHelper.FromVector2(forceDirection);
        float speedScale = Mathf.Clamp01(speedVector.magnitude / maxSpeed);
        float speedForceDot = Vector3.Dot(speedVector.normalized, forceVector);
        float forceScale = Mathf.Clamp(1f - speedScale * (1f + speedForceDot) / 2f, -1, 1);
        return forceVector * movementForce * forceScale;
    }

    void Dash(Vector2 forceDirection)
    {
        Quaternion orientation = Quaternion.Euler(0f, m_camera.rotation.eulerAngles.y, 0f);
        Vector3 forceVector = orientation * MathHelper.FromVector2(forceDirection);
        m_rigidbody.AddRelativeForce(forceVector * maxSpeedDash, ForceMode.VelocityChange);
        m_dashTimeout = new Timeout(dashCooldown);
        m_dashContinue = new Timeout(dashDuration);
        m_dashCharges--;
    }

    void Jump()
    {
        m_rigidbody.velocity = MathHelper.FlatVector(m_rigidbody.velocity);
        m_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        m_rigidbody.drag = dragAir;
        m_jumpTimeout = new Timeout(jumpCooldown);
        _jumpSound.Play();
        m_jumpCharges--;
    }

    Quaternion CursorDeltaToRotation(Vector3 delta)
    {
        return Quaternion.Euler(new Vector3(-delta.y * verticalSensitivity, delta.x * horizontalSensitivity, 0f));
    }

    private void UpdateMovement(InputState.CharacterInputState inputState)
    {
        m_rigidbody.AddForce(Direction2Dto3D(inputState.movementDirection));
        PlayRunSound(isGrounded && m_rigidbody.velocity.sqrMagnitude > 1.0f);
    }

    void PlayRunSound(bool playe)
    {
        if(_runSound.isPlaying)
        {
            if(!playe)
            {
                _runSound.Stop();
            }
        }
        else
        {
            if(playe)
            {
                _runSound.Play();
            }
        }
    }

    private void ClampSpeed()
    {
        Vector3 flatVelocity = MathHelper.FlatVector(m_rigidbody.velocity);
        float maxSpeed = maxSpeedGround;
        if (isDashing)
        {
            maxSpeed = maxSpeedDash;
            m_rigidbody.velocity = MathHelper.FlatVector(m_rigidbody.velocity);
        }
        if(flatVelocity.magnitude > maxSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxSpeed;
        }
        m_rigidbody.velocity = new Vector3(flatVelocity.x, m_rigidbody.velocity.y, flatVelocity.z);
    }

    //  isGrounded is late for one FixedStep. So we'll skip one step
    bool m_jumpedRecently = false;
    bool m_dashedRecently = false;

    void AddBonus(EPickupType pickupType)
    {
        switch (pickupType)
        {
            case EPickupType.Ammo:
                break;
            case EPickupType.Health:
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        var inputState = m_inputState.GetInputState();
        m_rigidbody.drag = drag;
        UpdateMovement(inputState);

        if(isGrounded && !m_jumpedRecently)
        {
            m_jumpCharges = jumpCharges;
        }
        m_jumpedRecently = false;

        if (isGrounded && !m_dashedRecently)
        {
            m_dashCharges = dashCharges;
        }
        m_dashedRecently = false;

        if (inputState.jump && CanJump())
        {
            Jump();
            m_jumpedRecently = true;
        }

        if(inputState.dash && CanDash())
        {
            Dash(inputState.movementDirection);
            m_dashedRecently = true;
        }

        jumpTestResultCache = null;
        ClampSpeed();
    }

    [Header("Player SOUNDS")]
    [Tooltip("Jump sound when player jumps.")]
    public AudioSource _jumpSound;
    [Tooltip("Sound while player makes when successfully reloads weapon.")]
    public AudioSource _freakingZombiesSound;
    [Tooltip("Sound Bullet makes when hits target.")]
    public AudioSource _hitSound;
    [Tooltip("Walk sound player makes.")]
    public AudioSource _walkSound;
    [Tooltip("Run Sound player makes.")]
    public AudioSource _runSound;
}
