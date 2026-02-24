using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Cài đặt")]
    public float tocDo = 5f;
    public float lucNhay = 5f;

    [Header("Cài đặt Nhảy")]
    public int soLanNhayToiDa = 2;
    private int soLanNhayConLai;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        soLanNhayConLai = soLanNhayToiDa;
    }

    public void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();
        moveX = move.x;
    }

    public void OnJump(InputValue value)
    {
        // Kiểm tra xem nút có bấm không
        if (value.isPressed)
        {
            if (soLanNhayConLai > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                // Nhảy
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, lucNhay);
                soLanNhayConLai--;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //kiểm tra chamj đất
        if (collision.gameObject.CompareTag("Ground"))
        {
            soLanNhayConLai = soLanNhayToiDa;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * tocDo, rb.linearVelocity.y);

        if (moveX != 0)
            anim.SetBool("isRunning", true);
        else
            anim.SetBool("isRunning", false);

        float currentScale = Mathf.Abs(transform.localScale.x);
        if (moveX > 0)
            transform.localScale = new Vector3(currentScale, transform.localScale.y, transform.localScale.z);
        else if (moveX < 0)
            transform.localScale = new Vector3(-currentScale, transform.localScale.y, transform.localScale.z);
    }
}