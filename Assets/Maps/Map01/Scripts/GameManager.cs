using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int health = 3;
    [SerializeField] private TextMeshProUGUI healthText ;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject Player;
    private bool isGameOver = false;
    private Animator anim;
    private void Awake()
    {
        anim = Player.GetComponent<Animator>();
    }


    void Start() { 
    
        UpdateHealth();
        gameOverUi.SetActive(false);
    }
     void Update()
    {
        
    }

    public void DecreaseHealth(int damage)
    {
       
        health=health-damage;
       
        if (health <= 0)
        {
            anim.SetTrigger("die");
            UpdateHealth();
            return;
        }
        anim.SetTrigger("hurt");
        UpdateHealth();

    }

    public int GetHealth()
    {
        return health;
    }

    public void IncreaseHealth ()
    {
        if (health==3) return;
        health++;
        UpdateHealth();
    }
    private void UpdateHealth ()
    {
        healthText.text = "Health: " + health.ToString();
    }

    public void gameOver() {

        isGameOver = true;
        Time.timeScale = 0;
        gameOverUi.SetActive(true);

    }
    public void RestartGame() {
        isGameOver = false;
        health = 3;
        UpdateHealth();
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public bool IsGameOver() {
        return isGameOver;
    }
   


}
