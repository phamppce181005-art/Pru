using UnityEngine;

public class PlayerConlision : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private Collider2D bodyCollider;
    private float lastHitTime;
    //[SerializeField] private float damageCooldown = 1f;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // chỉ xử lý nếu collider là thân player
        if (!collision.IsTouching(bodyCollider))
            return;

        if (collision.CompareTag("Bottle"))
        {
            Destroy(collision.gameObject);
            gameManager.IncreaseHealth();
        }
        else if (collision.CompareTag("Trap"))
        {
            gameManager.DecreaseHealth(1);
            Debug.Log("Player Health: Trap");
        }
        else if (collision.CompareTag("Enemy"))
        {
            gameManager.DecreaseHealth(1);
            Debug.Log("Player Health: Enemy");
             
        }
        else if (collision.CompareTag("Sea"))
        {
            gameManager.DecreaseHealth(3);
        }
    }

}
