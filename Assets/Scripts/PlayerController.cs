using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public bool DuringJump { get; private set; }
    public bool IsGrounded { get; private set; }

    public float MovementSpeed;
    public float MaxJumpHeight;
    public float MinJumpHeight;
    public float JumpMaxLoadingTime;
    public LayerMask GroundMask;
    public PhysicsMaterial2D PlayerMaterial, PlayerJumpMaterial;
    public GameObject spriteObject;

    Animator animator;
    Rigidbody2D rb;
    BoxCollider2D collider1;
    EdgeCollider2D collider2;
    float jumpLoadingTime = 0;
    bool waitAfterJump;
    bool duringLoadingJump;
    float moveInput = 0;

    



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider1 = GetComponent<BoxCollider2D>();
        collider2 = GetComponent<EdgeCollider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        moveInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(moveInput) < 0.02f)
            animator.SetBool("Walking", false);
        if (rb.velocity.x < -0.05f && spriteObject.transform.localScale.x > 0)
            Flip();
        else if (rb.velocity.x > 0.05f && spriteObject.transform.localScale.x < 0)
            Flip();
    }

    private void FixedUpdate()
    {
        if (!duringLoadingJump && IsGrounded && !DuringJump && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.velocity = new Vector2(MovementSpeed * moveInput, rb.velocity.y);
            if (Mathf.Abs(moveInput) > 0.02f)
                animator.SetBool("Walking", true);
        }
        if (!DuringJump)
            IsGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 0.15f), collider1.size, 0, GroundMask);
        else if (!waitAfterJump)
        {
            IsGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 0.4f), collider1.size * 0.7f, 0, GroundMask);
            animator.SetFloat("VelocityY", rb.velocity.y);
        }
        if (collider2.sharedMaterial == PlayerJumpMaterial && !waitAfterJump && IsGrounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Fall", true);
            Invoke("ResetFall", 0.5f);
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            collider2.sharedMaterial = PlayerMaterial;
            DuringJump = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded && !DuringJump)
        {
            animator.SetBool("Walking", false);
            duringLoadingJump = true;
            jumpLoadingTime = (float)context.startTime;
            rb.velocity = new Vector3(0, rb.velocity.y);
        }
        else if (context.canceled && IsGrounded && !DuringJump && duringLoadingJump)
        {
            collider2.sharedMaterial = PlayerJumpMaterial;
            rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
            jumpLoadingTime = (float)context.startTime - jumpLoadingTime;
            duringLoadingJump = false;
            DuringJump = true;
            waitAfterJump = true;
            float jumpX = moveInput > 0 ? 1 : -1;
            jumpX = moveInput == 0 ? 0 : jumpX;
            rb.velocity = new Vector2(MovementSpeed * jumpX / 1.2f, Mathf.Lerp(MinJumpHeight, MaxJumpHeight, jumpLoadingTime / JumpMaxLoadingTime));
            animator.SetBool("Jumping", true);
            Invoke("JumpCD", 0.2f);
        }
    }

    public void LoadDuringJump()
    {
        DuringJump = true;
        rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
        collider2.sharedMaterial = PlayerJumpMaterial;
    }

    void ResetFall()
    {
        animator.SetBool("Fall", false);
    }

    void Flip()
    {
        spriteObject.transform.localScale = new Vector3(spriteObject.transform.localScale.x * -1, spriteObject.transform.localScale.y, spriteObject.transform.localScale.z);
    }
    
    void JumpCD() => waitAfterJump = false;
    
}
