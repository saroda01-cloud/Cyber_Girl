using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float airControlMultiplier = 0.65f;
    public float jumpForce = 10f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Gravity Tuning")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private int jumpCount = 0;
    private bool isGrounded = false;
    private float moveInput;
    private Rigidbody2D playerRigidbody;
    private bool isDead = false;
    public DialogTest dialogTest;

    // 애니메이션 추가
    private Animator animator;

    // 보조 변수
    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // 애니메이터 가져오기
    }

    private void Update()
    {
        if (isDead) return;

        // 좌우 입력
        moveInput = Input.GetAxisRaw("Horizontal");

        // 코요테 타임 처리
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // 점프 버퍼 처리
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        // 점프 조건 (더블 점프 포함)
        if (jumpBufferTimer > 0f && (isGrounded || coyoteTimer > 0f || jumpCount < 2))
        {
            jumpCount++;
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, 0f);
            playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }

        // 가변 점프 (짧은 점프)
        if (Input.GetKeyUp(KeyCode.Space) && playerRigidbody.linearVelocity.y > 0f)
        {
            playerRigidbody.linearVelocity = new Vector2(
                playerRigidbody.linearVelocity.x,
                playerRigidbody.linearVelocity.y * 0.5f
            );
        }

        // 애니메이션 업데이트
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (dialogTest != null && dialogTest.IsInTalkRange)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
            return;
        }

        // 이동 (공중 제어력 적용)
        float control = isGrounded ? 1f : airControlMultiplier;
        playerRigidbody.linearVelocity = new Vector2(
            moveInput * moveSpeed * control,
            playerRigidbody.linearVelocity.y
        );

        // 중력 튜닝
        if (playerRigidbody.linearVelocity.y < 0)
        {
            playerRigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (playerRigidbody.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            playerRigidbody.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    // 애니메이션 업데이트 함수
    private void UpdateAnimation()
    {
        if (animator == null) return;

        // 이동 중인지 체크 (X축 속도가 0.1 이상이면 이동 중)
        bool isMoving = Mathf.Abs(playerRigidbody.linearVelocity.x) > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // 좌우 반전 (선택사항)
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // 오른쪽
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // 왼쪽
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.7f)
        {
            isGrounded = true;
            jumpCount = 0;
            // 착지 안정화
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, 0f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        playerRigidbody.linearVelocity = Vector2.zero;
        Debug.Log("Player Died!");
        Destroy(gameObject);
    }
}