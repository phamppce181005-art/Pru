using UnityEngine;

public class Medusa : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    [Header("Detection")]
    public float detectRange = 6f;
    public float attackRange = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallDistance = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

    private bool isFacingRight = true;
    private bool isDead = false;
    private bool isRunning = false;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    float distance;

    void Update()
    {
       

        distance = Vector2.Distance(transform.position, player.position);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= detectRange)
        {
            if (!isRunning)
            {
                anim.SetTrigger("Special");
                isRunning = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (distance <= detectRange)
        {
            RunToPlayer();
        }
        else
        {
            Patrol();
        }
    }
    void Patrol()
    {
        anim.SetBool("isRunning", false);
        anim.SetBool("isWalking", true);

        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);

        bool noGround = !Physics2D.Raycast(groundCheck.position, Vector2.down, groundDistance, groundLayer);
        bool hitWall = Physics2D.Raycast(wallCheck.position, Vector2.right * direction, wallDistance, groundLayer);

        if (noGround || hitWall)
        {
            Flip();
        }
    }

    void RunToPlayer()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", true);

        float direction = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * runSpeed, rb.linearVelocity.y);

        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
            Flip();
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isRunning", false);
        anim.SetBool("isWalking", false);

        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f; //  TẮT gravity
        rb.bodyType = RigidbodyType2D.Kinematic; //  Khóa vật lý

        anim.SetTrigger("Die");

        // Tắt collider sau 0.1s để animation kịp chạy
        Invoke(nameof(DisableCollider), 0.1f);

        Destroy(gameObject, 2f);
    }

    void DisableCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    public GameObject attackHitbox;

    public void EnableAttack()
    {
        attackHitbox.SetActive(true);
    }

    public void DisableAttack()
    {
        attackHitbox.SetActive(false);
    }
}