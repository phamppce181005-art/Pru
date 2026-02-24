using UnityEngine;

public class EnemyWeaponHitbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    private GameManager gameManager;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
                gameManager.DecreaseHealth(1);
            

        }
    }
}
