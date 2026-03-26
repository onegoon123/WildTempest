#define TOUCH

using Terresquall;
using UnityEngine;

public class PlayerMovement
{
    private Rigidbody2D rigidbody = null;
    private SpriteRenderer spriteRenderer = null;
    private Animator animator = null;
    private Vector2 moveDir;
    private Vector2 lastDir;
    public Vector2 MoveDir { get { return moveDir; } }
    public Vector2 LastDir { get { return lastDir; } }

    public float moveSpeed = 2.0f;
    public static float SpeedPercent = 1.0f;

    public PlayerMovement(Player player)
    {
        rigidbody = player.GetComponent<Rigidbody2D>();
        animator = player.GetComponentInChildren<Animator>();
        spriteRenderer = player.GetComponentInChildren<SpriteRenderer>();

        SpeedPercent = 1.0f;
    }

    public void Tick()
    {
        if (Time.timeScale == 0) { return; }
#if TOUCH
        moveDir = VirtualJoystick.GetInstance(0).GetAxis();
        bool isMove = !moveDir.Equals(Vector2.zero);
        animator.SetBool("Move", isMove);

        if (isMove)
        {
            lastDir = moveDir;
            if (0 < moveDir.x)
            {
                spriteRenderer.flipX = true;
            }
            else if (0 > moveDir.x)
            {
                spriteRenderer.flipX = false;
            }
        }
#endif
    }

    public void Tick_Fixed()
    {
        Vector2 moveVec = moveDir * moveSpeed * SpeedPercent * Time.fixedDeltaTime;
        rigidbody.MovePosition(rigidbody.position + moveVec);
    }

    public void OnMove(Vector2 moveDir)
    {
#if !TOUCH
        this.moveDir = moveDir;
        bool isMove = !moveDir.Equals(Vector2.zero);
        animator.SetBool("Move", isMove);

        if (isMove)
        {
            if (0 < moveDir.x)
            {
                spriteRenderer.flipX = false;
            }
            else if (0 > moveDir.x)
            {
                spriteRenderer.flipX = true;
            }
        }
#endif
    }
}
