using UnityEngine;

public class PlayerAnimationController
{
    private readonly PlayerController player;
    private readonly Animator animator;

    public PlayerAnimationController(PlayerController player, Animator animator)
    {
        this.player = player;
        this.animator = animator;
    }

    public void CheckMovingDirection()
    {
        if (!player.DuringJump && player.MoveInput < -0.05f && player.spriteObject.transform.localScale.x > 0)
            Flip();
        else if (!player.DuringJump && player.MoveInput > 0.05f && player.spriteObject.transform.localScale.x < 0)
            Flip();
    }

    public void CheckWalking()
    {
        if (player.DuringLoadingJump || player.DuringJump || Mathf.Abs(player.MoveInput) < 0.05f)
            animator.SetBool("Walking", false);
        if (!player.DuringLoadingJump && !player.DuringJump && Mathf.Abs(player.MoveInput) > 0.02f && Mathf.Abs(player.Rb.velocity.x) > 0.1f)
            animator.SetBool("Walking", true);
    }

    public void SetVelocity() => animator.SetFloat("VelocityY", player.Rb.velocity.y);

    public void LoadingJump() 
    { 
        animator.SetBool("Fall", false);
        animator.SetBool("LoadingJump", true); 
    }

    public void Jumped()
    {
        animator.SetBool("LoadingJump", false);
        animator.SetBool("Jumping", true);
    }

    public void Fell()
    {
        animator.SetBool("Jumping", false);
        animator.SetBool("Fall", true);
    }

    private void Flip() => player.spriteObject.transform.localScale = new Vector3(player.spriteObject.transform.localScale.x * -1, 
                   player.spriteObject.transform.localScale.y, player.spriteObject.transform.localScale.z);
}
