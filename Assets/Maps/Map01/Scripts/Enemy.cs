using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private AudioManaGer audioMana;

    public enum EnemyState { Patrol, Chase, Attack, Dead }
    private EnemyState currentState;
    [SerializeField] private GameObject weaponHitbox;
    [SerializeField] private Image healthFill;

    [Header("References")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectRange = 5f;
    public float attackRange = 2f;
    public float patrolDistance = 3f;

    [Header("Combat")]
    public int maxHealth = 3;
    public float attackCooldown = 1.5f;

    private int currentHealth;
    private bool isFacingRight = true;
    private bool canAttack = true;
    private bool isAttacking = false;


    private Vector2 startPos;

    /* ================= INIT ================= */
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioMana = FindAnyObjectByType<AudioManaGer>();


        currentHealth = maxHealth;
        startPos = transform.position;

        currentState = EnemyState.Patrol;
        weaponHitbox.SetActive(false);
    }

    /* ================= UPDATE ================= */

    void Update()
    {
        if (currentState == EnemyState.Dead) return;

        float distance =
            Vector2.Distance(transform.position, player.position);

        // lock state while attacking
        if (!isAttacking)
        {
            if (distance <= attackRange)
                currentState = EnemyState.Attack;
            else if (distance <= detectRange)
                currentState = EnemyState.Chase;
            else
                currentState = EnemyState.Patrol;
        }

        HandleState();
    }

    /* ================= STATE MACHINE ================= */

    void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    /* ================= PATROL ================= */

    void Patrol()
    {
        anim.SetBool("isWalking", true);

        Move(moveSpeed);

        float offset = transform.position.x - startPos.x;

        if (isFacingRight && offset >= patrolDistance)
            Flip();
        else if (!isFacingRight && offset <= -patrolDistance)
            Flip();
    }

    /* ================= CHASE ================= */

    /* ================= CHASE ================= */
    void Chase()
    {
        // XÓA đoạn if (distance <= attackRange) ở đây đi!

        anim.SetBool("isWalking", true);
        float dir = player.position.x - transform.position.x;
        if (dir > 0 && !isFacingRight) Flip();
        if (dir < 0 && isFacingRight) Flip();

        Move(moveSpeed);
    }

    /* ================= ATTACK ================= */
    void Attack()
    {
        StopMoving();
        if (!canAttack) return;

        canAttack = false;
        isAttacking = true; // Khóa state lại để không bị Chase đè lên

        anim.SetBool("isWalking", false);
        anim.SetTrigger("attack");
        audioMana.playEnemyAttack();
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
        isAttacking = false;
    }

    /* ================= DAMAGE ================= */

    public void TakeDamage(int dmg)
    {
        if (currentState == EnemyState.Dead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        anim.SetTrigger("isHurt");
        audioMana.playEnemyHurt();

        UpdateHealthBar();  

        if (currentHealth <= 0)
            Die();
    }
    void UpdateHealthBar()
    {
        if (healthFill == null) return;

        healthFill.fillAmount = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        currentState = EnemyState.Dead;

        StopMoving();

        anim.SetBool("isWalking", false);
        anim.SetTrigger("die");
        audioMana.playEnemyDie();

        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 2f);
    }

    /* ================= MOVEMENT ================= */

    void Move(float speed)
    {
        float dir = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale =
            new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    /* ================= GIZMOS ================= */

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void EnableHitbox()
    {
        weaponHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        weaponHitbox.SetActive(false);
    }
}
