using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private AudioManaGer audioMana;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;

    [SerializeField] private int maxHealth = 6;
    private int health = 6;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject gameWinUi;
    [SerializeField] private GameObject Player;
    private bool isGameOver = false;
    private bool isGameWin = false;
    private Animator anim;
    private void Awake()
    {
        anim = Player.GetComponent<Animator>();
        audioMana = FindAnyObjectByType<AudioManaGer>();

    }


    void Start() { 
    
        UpdateHealth();
        gameOverUi.SetActive(false);
        gameWinUi.SetActive(false);
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
            audioMana.playPlayerDie();

            UpdateHealth();
            return;
        }
        anim.SetTrigger("hurt");
        audioMana.playPlayerHurt();

        UpdateHealth();

    }

    public int GetHealth()
    {
        return health;
    }

    public void IncreaseHealth ()
    {
        if (health==6) return;
        health++;
        UpdateHealth();
    }
    private void UpdateHealth ()
    {
        for(int i = 0; i < hearts.Length; i++)
    {
            int heartValue = i * 2;

            if (health >= heartValue + 2)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (health == heartValue + 1)
            {
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    public void gameOver() {

        isGameOver = true;
        Time.timeScale = 0;
        gameOverUi.SetActive(true);

    }
    public void gameWin() {

        isGameWin = true;
        Time.timeScale = 0;
        gameWinUi.SetActive(true);

            }
    public void RestartGame() {
        StartCoroutine(RestartWithDelay());
    }
    private IEnumerator RestartWithDelay()
    {
        audioMana.playClick();

        yield return new WaitForSecondsRealtime(0.5f);
        isGameOver = false;
        health = 3;
        UpdateHealth();

        Time.timeScale = 1;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public bool IsGameOver() {
        return isGameOver;
    }
    public bool IsGameWin() {  return isGameWin; }

    public void goToMenu() {
        StartCoroutine(GoToMenuWithDelay());
    }
    private IEnumerator GoToMenuWithDelay()
    {
        audioMana.playClick();
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }


}
