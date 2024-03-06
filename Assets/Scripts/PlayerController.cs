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
    private bool canJump = true;
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

        IsGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y-0.5f), new Vector2(0.2f, 0.4f), 0, GroundMask);
        if (!duringLoadingJump && IsGrounded && canJump)
        {
            rb.velocity = new Vector2(MovementSpeed * moveInput, rb.velocity.y);
        }
        //Debug.Log(IsGrounded);
        if (collider1.sharedMaterial == PlayerJumpMaterial && canJump && IsGrounded)
        {
            rb.sharedMaterial = PlayerMaterial;
        }
        Debug.Log($"{IsGrounded}, {rb.velocity}         {Time.deltaTime}");
    }
 
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded)
        {
            duringLoadingJump = true;
            jumpLoadingTime = (float)context.startTime;
            rb.velocity = new Vector3(0, rb.velocity.y);
        }
        else if (context.canceled && IsGrounded && canJump && duringLoadingJump)
        {
            collider1.sharedMaterial = PlayerJumpMaterial;
            jumpLoadingTime = (float)context.startTime - jumpLoadingTime;
            duringLoadingJump = false;
            canJump = false;
            float jumpX = moveInput > 0 ? 1 : -1;
            jumpX = moveInput == 0 ? 0 : jumpX;
            rb.velocity = new Vector2(MovementSpeed * jumpX/1.2f, Mathf.Lerp(MinJumpHeight, MaxJumpHeight, jumpLoadingTime / JumpMaxLoadingTime));
            Invoke("JumpCD", 0.2f);
        }
    }
    void JumpCD() => canJump = true;

}
