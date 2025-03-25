using UnityEngine;

public class Wolf : MonoBehaviour
{
    public float speed = 3f;
    private Transform sheep;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public event System.Action OnWolfDestroyed;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject sheepObject = GameObject.FindGameObjectWithTag("Sheep");
        if (sheepObject != null)
        {
            sheep = sheepObject.transform;
        }
        else
        {
            Debug.LogError("❌ Sheep not found! Make sure the Sheep has the correct tag.");
        }

        if (animator == null) Debug.LogError("❌ Animator missing on Wolf!");
        if (spriteRenderer == null) Debug.LogError("❌ SpriteRenderer missing on Wolf!");
    }

    void Update()
    {
        if (sheep == null) return;

        transform.position = Vector2.MoveTowards(transform.position, sheep.position, speed * Time.deltaTime);
        Vector2 direction = (sheep.position - transform.position).normalized;
        spriteRenderer.flipX = direction.x < 0;

        if (animator != null)
        {
            animator.SetFloat("Blend", 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep"))
        {
            Debug.Log("Wolf caught the Sheep!");

            GameManagerSheep gameManager = FindObjectOfType<GameManagerSheep>();
            if (gameManager != null)
            {
                gameManager.OnWolfCollidesWithSheep();
            }
            else
            {
                Debug.LogError("GameManagerSheep not found in the scene!");
            }

            DestroyWolf();
        }
    }

    private void DestroyWolf()
    {
        OnWolfDestroyed?.Invoke();
        Destroy(gameObject); 
    }
}
