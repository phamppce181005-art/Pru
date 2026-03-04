using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack, Dead }
    private EnemyState currentState;
    private AudioManaGer audioMana;

    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject waterPrefab;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectRange = 7f;
    public float attackRange = 6f;
    public float patrolDistance = 3f;

    [Header("Combat")]
    public int maxHealth = 5;
    public float attackCooldown = 2f;

    private int currentHealth;
    private bool isFacingRight = true;
    private bool canAttack = true;
    private bool isAttacking = false;

    private GameObject currentBullet;
    private Vector2 startPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        audioMana = FindAnyObjectByType<AudioManaGer>();
        currentHealth = maxHealth;
        startPos = transform.position;
        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        if (currentState == EnemyState.Dead) return;

        float distance = Vector2.Distance(transform.position, player.position);

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

    void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase: Chase(); break;
            case EnemyState.Attack: Attack(); break;
        }
    }

    void Patrol()
    {
        anim.SetBool("isWalking", true);
        Move(moveSpeed);

        float offset = transform.position.x - startPos.x;

        if (isFacingRight && offset >= patrolDistance) Flip();
        else if (!isFacingRight && offset <= -patrolDistance) Flip();
    }

    void Chase()
    {
        anim.SetBool("isWalking", true);

        float dir = player.position.x - transform.position.x;

        if (dir > 0 && !isFacingRight) Flip();
        if (dir < 0 && isFacingRight) Flip();

        Move(moveSpeed);
    }

    void Attack()
    {
        StopMoving();

        if (!canAttack || currentBullet != null) return;

        canAttack = false;
        isAttacking = true;

        anim.SetBool("isWalking", false);
        anim.SetTrigger("attack");

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
        isAttacking = false;
    }

    // Animation Event gọi hàm này
    public void Shoot()
    {
        if (waterPrefab == null || firePoint == null) return;

        currentBullet = Instantiate(
            waterPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;

        currentBullet.GetComponent<WaterProjectile>()
            .Init(dir, this);
    }

    public void ClearBullet()
    {
        currentBullet = null;
    }

    public void TakeDamage(int dmg)
    {
        if (currentState == EnemyState.Dead) return;

        currentHealth -= dmg;

        anim.SetTrigger("isHurt");
        audioMana.playEnemyHurt();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        currentState = EnemyState.Dead;

        StopMoving();
        anim.SetTrigger("die");

        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 2f);
    }

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
}