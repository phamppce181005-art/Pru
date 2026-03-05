using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private AudioManaGer audioMana;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float slideTime = 0.5f;
    [SerializeField] private GameObject weaponHitbox;
    [SerializeField] private Transform groundCheckLeft;
    [SerializeField] private Transform groundCheckRight;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;


    [Header("Climb")]
    [SerializeField] private float climbSpeed = 4f;

    private bool isGrabbing;
    private bool isClimbing;

    [Header("Attack")]
    [SerializeField] private float comboResetTime = 0.8f;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool isSliding;

    private float slideTimer;


    // ===== COMBO SYSTEM =====
    private int comboStep = 0;                 // 0 = idle
    private bool attackQueued = false;         // buffer click
    private bool canReceiveAttackInput = true; // chống spam
    private float lastAttackTime;
    private GameManager gameManager;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        weaponHitbox.SetActive(false);
        audioMana = FindAnyObjectByType<AudioManaGer>();    
    }

    void Update()
    {
        if (gameManager.IsGameOver()||gameManager.IsGameWin()) return;
       
        HandleSlide();
        HandleComboTimeout();
        HandleClimb();
        FlipPlayer();
        UpdateAnimator();
        
    }

    void FixedUpdate()
    {
        CheckGround();
        if (!isSliding && !isGrabbing)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    /* ================= INPUT ================= */
    void CheckGround()
    {
        bool leftGrounded = Physics2D.OverlapCircle(
           groundCheckLeft.position,
           groundCheckRadius,
           groundLayer
       );

        bool rightGrounded = Physics2D.OverlapCircle(
            groundCheckRight.position,
            groundCheckRadius,
            groundLayer
        );

        isGrounded = leftGrounded || rightGrounded;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!isGrounded) return;
        audioMana.playJump();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!isGrounded || isSliding) return;

        isSliding = true;
        slideTimer = slideTime;

        float dir = transform.localScale.x;
        audioMana.playSlide();
        rb.linearVelocity = new Vector2(dir * slideSpeed, rb.linearVelocity.y);

        anim.SetBool("isSliding", true);
    }

    // 🖱 ATTACK INPUT (ANTI-SPAM)
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isSliding) return;
        if (!canReceiveAttackInput) return;

        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }

        // CHƯA ĐÁNH → ATTACK 1
        if (comboStep == 0)
        {
            comboStep = 1;
            anim.SetInteger("comboStep", comboStep);
            anim.SetTrigger("attack");
            //audioMana.playAttack();
        }
        // ĐANG ĐÁNH → QUEUE ĐÒN KẾ
        else
        {
            attackQueued = true;
            anim.SetBool("attackQueued", true);
            canReceiveAttackInput = false; // khóa input tới cuối animation
        }

        lastAttackTime = Time.time;
    }

    /* ================= LOGIC ================= */

    void HandleSlide()
    {
        if (!isSliding) return;

        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0)
        {
            isSliding = false;
            anim.SetBool("isSliding", false);
        }
    }

    void HandleComboTimeout()
    {
        if (comboStep == 0) return;

        if (Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }
    void HandleClimb()
    {
        if (isGrabbing)
        {
            rb.gravityScale = 0f;

            float vertical = moveInput.y;

            if (vertical != 0)
            {
                isClimbing = true;
                rb.linearVelocity = new Vector2(0, vertical * climbSpeed);
            }
            else
            {
                isClimbing = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            rb.gravityScale = 5f; // hoặc giá trị gravity cũ của bạn
            isClimbing = false;
        }

        anim.SetBool("isGrabbing", isGrabbing);
        anim.SetBool("isClimbing", isClimbing);
        anim.SetFloat("climbY", moveInput.y);
    }

    // 🎯 GỌI TỪ Animation Event (CUỐI Attack1 / Attack2 / Attack3)
    public void EndAttack()
    {
        if (attackQueued && comboStep < 3)
        {
            comboStep++;
            anim.SetInteger("comboStep", comboStep);
        }
        else
        {
            ResetCombo();
        }

        attackQueued = false;
        anim.SetBool("attackQueued", false);
        canReceiveAttackInput = true; // mở lại input
    }

    void ResetCombo()
    {
        comboStep = 0;
        anim.SetInteger("comboStep", 0);
        attackQueued = false;
        anim.SetBool("attackQueued", false);
        canReceiveAttackInput = true;
    }

    /* ================= UTILS ================= */

    void FlipPlayer()
    {
        if (moveInput.x > 0)
            transform.localScale = Vector3.one;
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void UpdateAnimator()
    {
        anim.SetFloat("moveX", Mathf.Abs(moveInput.x));
        anim.SetBool("isGrounded", isGrounded);
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        isGrounded = true;
    //    }
    //}


    public void EnableHitbox()
    {
        audioMana.playAttack();
        weaponHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        weaponHitbox.SetActive(false);
    }
    public void OnPlayerDeathAnimationEnd()
    {
        gameManager.gameOver();
    }

    // ===== LADDER TRIGGER =====

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") && !isGrounded)
        {
            isGrabbing = true;

            rb.linearVelocity = Vector2.zero;

            // Snap vào giữa thang
            transform.position = new Vector3(
                collision.transform.position.x,
                transform.position.y,
                transform.position.z
            );
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isGrabbing = false;
            isClimbing = false;
        }
    }
}
