using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public float MovementSpeed;
    public float MaxJumpHeight;
    public float MinJumpHeight;
    public float JumpMaxLoadingTime;
    public LayerMask GroundMask;
    public PhysicsMaterial2D PlayerMaterial, PlayerJumpMaterial;

    public bool IsGrounded;

    Rigidbody2D rb;
    BoxCollider2D collider1;
    float jumpLoadingTime = 0;
    private bool duringJump = false;
    private bool waitAfterJump = false;
    private bool duringLoadingJump = false;
    float moveInput = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider1 = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (!duringLoadingJump && IsGrounded && !duringJump)
        {
            rb.velocity = new Vector2(MovementSpeed * moveInput, rb.velocity.y);
        }
        
        Debug.Log($"{IsGrounded}, {rb.velocity}         {Time.deltaTime}");
    }
    private void FixedUpdate()
    {
        if(!duringJump)
            IsGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y-0.05f), collider1.size, 0, GroundMask);
        else if (!waitAfterJump)
            IsGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y-0.25f), collider1.size*0.8f, 0, GroundMask);
        if (collider1.sharedMaterial == PlayerJumpMaterial && !waitAfterJump && IsGrounded)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            collider1.sharedMaterial = PlayerMaterial;
            duringJump = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded)
        {
            duringLoadingJump = true;
            jumpLoadingTime = (float)context.startTime;
            rb.velocity = new Vector3(0, rb.velocity.y);
        }
        else if (context.canceled && IsGrounded && !duringJump && duringLoadingJump)
        {
            collider1.sharedMaterial = PlayerJumpMaterial;
            rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
            jumpLoadingTime = (float)context.startTime - jumpLoadingTime;
            duringLoadingJump = false;
            duringJump = true;
            waitAfterJump = true;
            float jumpX = moveInput > 0 ? 1 : -1;
            jumpX = moveInput == 0 ? 0 : jumpX;
            rb.velocity = new Vector2(MovementSpeed * jumpX/1.2f, Mathf.Lerp(MinJumpHeight, MaxJumpHeight, jumpLoadingTime / JumpMaxLoadingTime));
            Invoke("JumpCD", 0.2f);
        }
    }
    void JumpCD() => waitAfterJump = false;
}
