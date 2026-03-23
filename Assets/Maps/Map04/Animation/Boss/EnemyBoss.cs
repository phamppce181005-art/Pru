using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public Transform player;

    [Header("HP")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Movement")]
    public float speed = 2f;
    public float patrolDistance = 3f;
    private Vector3 startPos;
    private bool movingRight = true;

    [Header("Detection")]
    public float detectRange = 6f;
    public float attackRange = 2f;

    [Header("Fly")]
    public float flySpeed = 4f;
    public float flyHeight = 2f;

    [Header("Attack")]
    public float attackCooldown = 2f;
    private float lastAttackTime;
    private bool isAttacking = false;

    [Header("Attack Hitbox")]
    public GameObject attackHitbox;

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        startPos = transform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 🔥 CHECK CHẾT NGAY (giống Medusa)
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (isDead) return;

        if (distance <= detectRange)
        {
            AttackBehavior(distance);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        isAttacking = false;

        anim.SetBool("isWalking", true);
        anim.SetBool("fly", false);

        float move = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * move * speed * Time.deltaTime);

        if (movingRight && transform.position.x >= startPos.x + patrolDistance)
            Flip();
        else if (!movingRight && transform.position.x <= startPos.x - patrolDistance)
            Flip();
    }

    void AttackBehavior(float distance)
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("fly", true);

        Vector3 targetPos = new Vector3(
            player.position.x,
            player.position.y + flyHeight,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            flySpeed * Time.deltaTime
        );

        if (player.position.x > transform.position.x && !movingRight)
            Flip();
        else if (player.position.x < transform.position.x && movingRight)
            Flip();

        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            if (distance <= attackRange)
                anim.SetTrigger("attack1");
            else
                anim.SetTrigger("attack2");

            isAttacking = true;
            lastAttackTime = Time.time;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    // 🔥 NHẬN DAMAGE (FIX CHUẨN)
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log("Boss HP: " + currentHealth);

        anim.SetTrigger("hurt"); // giống Medusa

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 💀 DIE (FIX GIỐNG MEDUSA)
    void Die()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("die");

        // Dừng hoàn toàn
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Tắt collider sau 1 chút để animation chạy
        Invoke(nameof(DisableCollider), 0.1f);

        Destroy(gameObject, 3f);
    }

    void DisableCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    void Flip()
    {
        if (isDead) return;

        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void EnableAttack()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    public void DisableAttack()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }
}