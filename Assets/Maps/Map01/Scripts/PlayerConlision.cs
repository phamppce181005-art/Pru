using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerConlision : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManaGer audioMana;
    [SerializeField] private Collider2D bodyCollider;
    private float lastHitTime;
    //[SerializeField] private float damageCooldown = 1f;
    //private int chapter = 1;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioMana = FindAnyObjectByType<AudioManaGer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // chỉ xử lý nếu collider là thân player
        if (!collision.IsTouching(bodyCollider))
            return;

        if (collision.CompareTag("Bottle"))
        {
            Destroy(collision.gameObject);
            audioMana.playHealthBottle();
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
            gameManager.DecreaseHealth(6);
        }
        else if (collision.CompareTag("Flag"))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;

            
            if (currentIndex == 5)
            {
                gameManager.gameWin();
            }
            else
            {
                SceneManager.LoadScene(currentIndex + 1);
            }
        }
    }

}
