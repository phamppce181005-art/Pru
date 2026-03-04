using UnityEngine;

public class WaterProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;

    private Vector2 direction;
    private BossEnemy owner;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void Init(Vector2 dir, BossEnemy boss)
    {
        direction = dir;
        owner = boss;

        // Lật sprite theo hướng bay
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.DecreaseHealth(1);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (owner != null)
            owner.ClearBullet();
    }
}