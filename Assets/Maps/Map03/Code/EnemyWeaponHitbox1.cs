using UnityEngine;

public class EnemyWeaponHitbox1 : MonoBehaviour
{
    private GameManager gameManager;
    private bool hasHit = false;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        hasHit = false; // Reset mỗi lần chém
        Debug.Log("HitBox được bật!"); // Kiểm tra có bật đúng lúc không
    }

    private void OnDisable()
    {
        Debug.Log("HitBox được tắt!"); // Kiểm tra có tắt đúng lúc không
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasHit)
        {
            hasHit = true;
            gameManager.DecreaseHealth(1);
            Debug.Log("Player bị trúng đòn!");
        }
    }
}