using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 2f;
    public Collider2D boundaryArea; // Use Collider2D instead of RectTransform
    public LayerMask obstacleLayer;
    private Vector2 targetPosition;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        SetNewTarget();
    }

    void Update()
    {
        MoveFish();
    }

    void MoveFish()
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Update Animator
        Vector2 normalizedDirection = direction.normalized;
        animator.SetFloat("MoveX", normalizedDirection.x);
        animator.SetFloat("MoveY", normalizedDirection.y);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTarget();
        }
    }

    void SetNewTarget()
    {
        if (boundaryArea == null)
        {
            Debug.LogError("Boundary Area not set!");
            return;
        }

        Bounds bounds = boundaryArea.bounds;

        int attempts = 10;
        for (int i = 0; i < attempts; i++)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            Vector2 newTarget = new Vector2(x, y);

            // Ensure target is inside boundary and not inside an obstacle
            if (!Physics2D.OverlapCircle(newTarget, 0.5f, obstacleLayer))
            {
                targetPosition = newTarget;
                return;
            }
        }

        Debug.LogWarning("No valid target found! Using center of boundary.");
        targetPosition = bounds.center;
    }
}
