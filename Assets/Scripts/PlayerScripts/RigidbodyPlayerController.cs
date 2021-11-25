using Assets.Scripts.Server.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyPlayerController : MonoBehaviour, INetInputReceiver
{
    private const float GROUND_CHECK_SPHERE_OFFSET = 0.05f;

    [SerializeField] private Transform cameraTransform;

    [Space]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private CapsuleCollider playerCollider;

    [Space]
    [SerializeField] private LayerMask groundMask = 1;
    [SerializeField] private float groundCheckDistance = 0.1f;

    [Space]
    public float jumpForce = 5;
    public int maxJumps = 1;
    public float walkSpeed = 3;
    public float runSpeed = 5;

    [Space]
    public float friction = 10f;
    public float minVelocity = 0.1f;
    public float walkToRunTransitionTime = 0.3f;

    [Space]
    public bool debug;

    private int jumpsLeft;
    private bool isGrounded;

    private bool pressedJump;
    private float timeInSprint;

    private bool isSprinting;

    void Start()
    {
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        //if (Input.GetButtonDown("Jump"))
        //    pressedJump = true;
        //else if (Input.GetButtonUp("Jump"))
        //    pressedJump = false;

        //isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    public void Tick(INetInputSource source)
    {        
        pressedJump = source.GetButtonDown("Jump");
        isSprinting = source.GetButton("Sprint");

        // NOTE:
        // Applying physics to the rigidbody must be in FixedUpdate.
        // Otherwise it will cause (jittery) / (unpredictable) movements.
        RotateBodyToLookingDirection();

        isGrounded = IsGrounded();

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
        }

        if (pressedJump)
        {
            pressedJump = false;

            if (isGrounded || jumpsLeft > 0)
            {
                Jump();
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            float rot = Input.GetAxisRaw("Vertical");
            //orientation.Rotate(Vector3.right * 2 * rot * Time.fixedDeltaTime);
        }
        else Movement(source);
    }

    private void RotateBodyToLookingDirection()
    {
        Vector3 euler = cameraTransform.eulerAngles;
        euler.x = euler.z = 0;
        transform.eulerAngles = euler;
    }

    private void Jump()
    {
        Vector3 currentVelocity = playerRigidbody.velocity;
        currentVelocity.y = 0;
        playerRigidbody.velocity = currentVelocity;

        playerRigidbody.AddForce(jumpForce * Vector3.up, ForceMode.VelocityChange);
        jumpsLeft--;
    }

    private void Movement(INetInputSource source)
    {
        Vector3 sidewaysVector = transform.right * source.GetAxisRaw("Horizontal");
        Vector3 forwardVector = transform.forward * source.GetAxisRaw("Vertical");
        Vector3 direction = (sidewaysVector + forwardVector).normalized;

        AddMovementForce(direction, LerpSpeed());

        if (isGrounded)
        {
            FrictionForces();
        }
    }

    private float LerpSpeed()
    {
        if (walkToRunTransitionTime == 0)
            return isSprinting ? runSpeed : walkSpeed;

        if (isSprinting)
            timeInSprint += Time.fixedDeltaTime;
        else
            timeInSprint -= Time.fixedDeltaTime;

        timeInSprint = Mathf.Clamp(timeInSprint, 0, walkToRunTransitionTime);
        float lerpedSpeed = walkSpeed + Mathf.Lerp(walkSpeed, runSpeed, timeInSprint / walkToRunTransitionTime);
        return lerpedSpeed;
    }

    private void AddMovementForce(Vector3 direction, float speed)
    {
        Vector3 curentVelocity = playerRigidbody.velocity;
        curentVelocity.y = 0;

        playerRigidbody.AddForce(speed * direction, ForceMode.VelocityChange);

        if (curentVelocity.magnitude > walkSpeed)
        {
            float multiplier = walkSpeed / curentVelocity.magnitude;
            playerRigidbody.velocity = curentVelocity * multiplier + Vector3.up * playerRigidbody.velocity.y;
        }
    }

    private void FrictionForces()
    {
        if (playerRigidbody.velocity.magnitude > minVelocity)
            playerRigidbody.AddForce(-1 * friction * GetHorizontalVelocity().normalized, ForceMode.Force);
        else
            playerRigidbody.velocity = Vector3.zero;
    }

    private Vector3 FeetPosition()
    {
        Vector3 sphereOffset = (playerCollider.height * transform.localScale.y / 2 - playerCollider.radius * transform.localScale.x) * -1 * transform.up;
        Vector3 feetPosition = playerRigidbody.position + sphereOffset;
        return feetPosition;
    }

    public Vector3 GetHorizontalVelocity()
    {
        Vector3 horizontalVelocity = playerRigidbody.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity;
    }

    public bool IsGrounded()
    {
        Vector3 upOffset = transform.up * GROUND_CHECK_SPHERE_OFFSET;
        bool isGrounded = Physics.SphereCast(FeetPosition() + upOffset,
            playerCollider.radius * transform.localScale.x, -1 * transform.up, out RaycastHit info,
            groundCheckDistance + GROUND_CHECK_SPHERE_OFFSET, groundMask);
        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Vector3 feetOffset = -1 * groundCheckDistance * transform.up;
            Gizmos.DrawWireSphere(FeetPosition() + feetOffset, playerCollider.radius * transform.localScale.x);
        }
    }
}
