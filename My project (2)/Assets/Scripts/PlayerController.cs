using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private float moveInput;
    private bool isInvincible = false;

    private float originalSpeed;
    private float originalJumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalSpeed = moveSpeed;
        originalJumpForce = jumpForce;
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, .2f, groundLayer);

        // --- 모션 방향 수정 (백스텝 해결) ---
        // 캐릭터가 기본적으로 왼쪽을 보고 있다면:
        // 오른쪽(moveInput > 0)으로 갈 때 스케일을 -1로 반전시켜야 앞을 봅니다.
        if (moveInput > 0) transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (moveInput < 0) transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            if (!isInvincible)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }

        if (collision.CompareTag("Enemy"))
        {
            if (!isInvincible)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (collision.CompareTag("Speed"))
        {
            StartCoroutine(SpeedUpRoutine());
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("jump"))
        {
            StartCoroutine(JumpUpRoutine());
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("shild"))
        {
            StartCoroutine(PowerUpRoutine());
            Destroy(collision.gameObject);
        }
    }

    IEnumerator JumpUpRoutine()
    {
        jumpForce = originalJumpForce + 3f;
        yield return new WaitForSeconds(5f);
        jumpForce = originalJumpForce;
    }

    IEnumerator SpeedUpRoutine()
    {
        moveSpeed = originalSpeed + 3f;
        yield return new WaitForSeconds(5f);
        moveSpeed = originalSpeed;
    }

    IEnumerator PowerUpRoutine()
    {
        isInvincible = true;

        float timer = 5f;
        while (timer > 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(0.1f);
            timer -= 0.2f;
        }

        ResetPowerUp();
    }

    private void ResetPowerUp()
    {
        isInvincible = false;
        spriteRenderer.color = new Color(1, 1, 1, 1f);
    }
}
