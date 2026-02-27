using UnityEngine;

public class AudioManaGer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private AudioSource background;
    [SerializeField] private AudioSource effect;

    [SerializeField] private AudioClip backgroundclip    ;
    [SerializeField] private AudioClip jumpclip;
    [SerializeField] private AudioClip attackclip;
    [SerializeField] private AudioClip slideclip;
    [SerializeField] private AudioClip bottleclip;
    [SerializeField] private AudioClip hurtclip;
    [SerializeField] private AudioClip dieclip;
    [SerializeField] private AudioClip enemydieclip;
    [SerializeField] private AudioClip enemyhurtclip;
    [SerializeField] private AudioClip enemyattackclip;
    [SerializeField] private AudioClip clickclip;


    void Start()
    {
        playBackground();
    }

    // Update is called once per frame
   
    public void playBackground() { 
        background.clip= backgroundclip ;
        background.Play();

    }
    public void playJump() {

        effect.PlayOneShot(jumpclip);
    }
    public void playAttack() {

        effect.PlayOneShot(attackclip);
    }
    public void playPlayerHurt() {

        effect.PlayOneShot(hurtclip);
    }
    public void playPlayerDie() {

        effect.PlayOneShot(dieclip);
    }
    public void playEnemyHurt() {

        effect.PlayOneShot(enemyhurtclip);
    }
    public void playEnemyDie() {

        effect.PlayOneShot(enemydieclip);
    }
    public void playHealthBottle() {

        effect.PlayOneShot(bottleclip);
    }
    public void playSlide() {

        effect.PlayOneShot(slideclip);
    }
    public void playEnemyAttack() {

        effect.PlayOneShot(enemyattackclip);
    }
    public void playClick() {

        effect.PlayOneShot(clickclip);
    }
}
