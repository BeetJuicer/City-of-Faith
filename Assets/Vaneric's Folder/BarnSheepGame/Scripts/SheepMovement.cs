using UnityEngine;
using System.Collections;

public class SheepMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Animator animator;
    private bool isMoving = false;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private Vector2[] directions = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        UpdateBounds();
        StartCoroutine(MovementRoutine());
    }

    void UpdateBounds()
    {
        minBounds = Camera.main.ViewportToWorldPoint(Vector2.zero);
        maxBounds = Camera.main.ViewportToWorldPoint(Vector2.one);
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPosition = rb.position + movementDirection * moveSpeed * Time.fixedDeltaTime;
            newPosition = ClampToBounds(newPosition);
            rb.MovePosition(newPosition);
        }

        UpdateAnimation();
    }

    IEnumerator MovementRoutine()
    {
        while (true)
        {
            if (Random.value < 0.9f) // 90% chance to move
            {
                ChooseDirection();
                isMoving = true;
                yield return new WaitForSeconds(Random.Range(2f, 4f));

                if (IsNearBoundary(rb.position))
                {
                    MoveTowardCenter();
                    yield return new WaitForSeconds(Random.Range(2f, 5f));
                }
            }
            else
            {
                isMoving = false;
                yield return new WaitForSeconds(Random.Range(1f, 3f)); // Idle time
            }
        }
    }

    void ChooseDirection()
    {
        Vector2 currentPos = rb.position;

        if (IsNearBoundary(currentPos))
        {
            movementDirection = AvoidBoundary(currentPos);
        }
        else
        {
            movementDirection = MoveTowardCenter();
        }
    }

    Vector2 MoveTowardCenter()
    {
        Vector2 centerPosition = (minBounds + maxBounds) / 2; // Get the middle of the screen
        Vector2 centerDirection = (centerPosition - rb.position).normalized;

        if (Mathf.Abs(centerDirection.x) > Mathf.Abs(centerDirection.y))
        {
            movementDirection = centerDirection.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            movementDirection = centerDirection.y > 0 ? Vector2.up : Vector2.down;
        }

        return movementDirection;
    }

    bool IsNearBoundary(Vector2 position)
    {
        return position.x <= minBounds.x + 2f || position.x >= maxBounds.x - 2f ||
               position.y <= minBounds.y + 2f || position.y >= maxBounds.y - 2f;
    }

    Vector2 AvoidBoundary(Vector2 position)
    {
        Vector2 away = Vector2.zero;

        if (position.x <= minBounds.x + 2f) away += Vector2.right;
        if (position.x >= maxBounds.x - 2f) away += Vector2.left;
        if (position.y <= minBounds.y + 2f) away += Vector2.up;
        if (position.y >= maxBounds.y - 2f) away += Vector2.down;

        return away.normalized;
    }

    Vector2 ClampToBounds(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x, minBounds.x + 1f, maxBounds.x - 1f);
        position.y = Mathf.Clamp(position.y, minBounds.y + 1f, maxBounds.y - 1f);
        return position;
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetFloat("MoveX", movementDirection.x);
        animator.SetFloat("MoveY", movementDirection.y);
        animator.SetBool("IsMoving", isMoving);
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fence"))
        {
            ChooseDirection(); // Change direction when hitting fence
        }
        else if (collision.gameObject.CompareTag("Sheep"))
        {
            Vector2 pushDirection = (rb.position - (Vector2)collision.transform.position).normalized;
            rb.MovePosition(rb.position + pushDirection * 0.3f);
        }
    }
}
