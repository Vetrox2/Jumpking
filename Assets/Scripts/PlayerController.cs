using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public bool DuringJump { get; private set; }
    public bool IsGrounded { get; private set; }
    public float MoveInput { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public bool DuringLoadingJump { get; private set; }

    public GameObject spriteObject;

    [SerializeField]
    private float MovementSpeed;
    [SerializeField]
    private float MaxJumpHeight;
    [SerializeField]
    private float MinJumpHeight;
    [SerializeField]
    private float JumpMaxLoadingTime;
    [SerializeField]
    private LayerMask GroundMask;
    [SerializeField]
    private PhysicsMaterial2D PlayerMaterial, PlayerJumpMaterial;

    private BoxCollider2D collider1;
    private EdgeCollider2D collider2;
    private float jumpLoadingTime;
    private bool waitAfterJump;
    private PlayerAnimationController animController;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        collider1 = GetComponent<BoxCollider2D>();
        collider2 = GetComponent<EdgeCollider2D>();
        animController = new(this, GetComponentInChildren<Animator>());
    }

    private void Update()
    {
        MoveInput = Input.GetAxis("Horizontal");
        animController.CheckWalking();
        animController.CheckMovingDirection();
        animController.SetVelocity();
    }

    private void FixedUpdate()
    {
        if (!DuringLoadingJump && IsGrounded && !DuringJump && Mathf.Abs(Rb.velocity.y) < 0.1f)
        {
            Rb.velocity = new Vector2(MovementSpeed * MoveInput, Rb.velocity.y);
        }
        if (!DuringJump)
            IsGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 0.15f), collider1.size, 0, GroundMask);
        else if (!waitAfterJump)
        {
            IsGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 0.4f), collider1.size * 0.7f, 0, GroundMask);
        }
        if (collider2.sharedMaterial == PlayerJumpMaterial && !waitAfterJump && IsGrounded)
        {
            animController.Fell();
            Rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            collider2.sharedMaterial = PlayerMaterial;
            DuringJump = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded && !DuringJump)
        {
            animController.LoadingJump();
            DuringLoadingJump = true;
            jumpLoadingTime = (float)context.startTime;
            Rb.velocity = new Vector3(0, Rb.velocity.y);
        }
        else if (context.canceled && IsGrounded && !DuringJump && DuringLoadingJump)
        {
            animController.Jumped();
            collider2.sharedMaterial = PlayerJumpMaterial;
            Rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
            jumpLoadingTime = (float)context.startTime - jumpLoadingTime;
            DuringLoadingJump = false;
            DuringJump = true;
            waitAfterJump = true;
            float jumpX = MoveInput > 0 ? 1 : -1;
            jumpX = MoveInput == 0 ? 0 : jumpX;
            Rb.velocity = new Vector2(MovementSpeed * jumpX / 1.2f, Mathf.Lerp(MinJumpHeight, MaxJumpHeight, jumpLoadingTime / JumpMaxLoadingTime));
            Invoke("JumpCD", 0.2f);
        }
    }

    public void LoadDuringJump()
    {
        DuringJump = true;
        Rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
        collider2.sharedMaterial = PlayerJumpMaterial;
    }

    private void JumpCD() => waitAfterJump = false;
}
