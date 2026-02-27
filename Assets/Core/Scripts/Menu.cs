using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private AudioManaGer audioMana;

   void Awake()
    {
        audioMana = FindAnyObjectByType<AudioManaGer>();

    }
    public void playGame() {
        audioMana.playClick();
        SceneManager.LoadScene("Game");
        
    }
    private IEnumerator PlayWithDelay()
    {
        audioMana.playClick();
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }
    public void stopGame() {
        audioMana.playClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
