using UnityEngine;

public class TapSheep : MonoBehaviour
{
    private GameControllerSheep gameController;
    private bool isTapped = false;

    void Start()
    {
        gameController = FindObjectOfType<GameControllerSheep>();
    }

    void OnMouseDown()
    {
        if (Time.timeScale == 0 || isTapped) return;
        CatchThisSheep();
    }

    void Update()
    {
        if (Time.timeScale == 0 || Input.touchCount == 0) return;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    CatchThisSheep();
                }
            }
        }
    }

    void CatchThisSheep()
    {
        if (isTapped) return; // Prevent double-tap
        isTapped = true;

        if (gameController != null)
        {
            gameController.CatchSheep(gameObject);
        }
    }
}