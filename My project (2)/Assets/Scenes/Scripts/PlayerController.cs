using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private float moveInput;

    // 더블 점프를 위한 변수 추가
    private int jumpCount = 0;
    public int maxJumpCount = 2; // 2로 설정하면 더블 점프 가능

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput < 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput > 0)
            transform.localScale = new Vector3(-1, 1, 1);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .2f, groundLayer);

        // 바닥에 닿으면 점프 횟수 초기화
        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            jumpCount = 0;
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump(InputValue value)
    {
        // 버튼을 눌렀을 때만 실행
        if (value.isPressed)
        {
            // 바닥에 있거나, 점프 횟수가 최대치보다 적을 때 실행
            if (isGrounded || jumpCount < maxJumpCount)
            {
                // 공중 점프 시에도 일정한 높이를 위해 y축 속도 초기화
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                pAni.SetTrigger("Jump");
                jumpCount++; // 점프 횟수 증가
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }

        if (collision.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
