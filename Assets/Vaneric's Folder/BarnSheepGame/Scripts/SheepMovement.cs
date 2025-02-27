using UnityEngine;
using System.Collections;

public class SheepMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Animator animator;
    private bool isMoving = false;

    private float centerBias = 0.7f;
    private Vector2[] directions = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(ChangeDirectionRoutine());
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPosition = rb.position + movementDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }

        UpdateAnimationState();
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            ChangeDirection();
            isMoving = true;
            yield return new WaitForSeconds(Random.Range(2f, 4f)); // Move for a few seconds
            isMoving = false;
            yield return new WaitForSeconds(Random.Range(1f, 2f)); // Pause before changing direction
        }
    }

    void ChangeDirection()
    {
        Vector2 position = rb.position;

        if (position.x < -3 && Random.value < centerBias) movementDirection = Vector2.right;
        else if (position.x > 3 && Random.value < centerBias) movementDirection = Vector2.left;
        else if (position.y < -3 && Random.value < centerBias) movementDirection = Vector2.up;
        else if (position.y > 3 && Random.value < centerBias) movementDirection = Vector2.down;
        else movementDirection = directions[Random.Range(0, directions.Length)];
    }

    void UpdateAnimationState()
    {
        if (animator == null) return;

        if (isMoving)
        {
            animator.SetFloat("MoveX", movementDirection.x);
            animator.SetFloat("MoveY", movementDirection.y);
        }
        else
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
        }
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fence")) 
        {
            StartCoroutine(ForceChangeDirection());
        }
    }

    IEnumerator ForceChangeDirection()
    {
        int attempts = 0;

        while (attempts < 5) 
        {
            ChangeDirection();
            yield return new WaitForSeconds(0.1f);

            if (!IsBlocked()) break; 

            attempts++;
        }
    }

    bool IsBlocked()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, movementDirection, 0.5f);
        return hit.collider != null && hit.collider.CompareTag("Fence");
    }
}
