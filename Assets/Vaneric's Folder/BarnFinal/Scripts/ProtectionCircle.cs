using UnityEngine;
using UnityEngine.EventSystems;

public class ProtectionCircle : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private float minX, maxX, minY, maxY;
    private float circleRadius;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
        circleRadius = GetComponent<CircleCollider2D>().radius * transform.lossyScale.x;
    }

    void Update()
    {
        SetScreenBoundaries();
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.touchSupported && Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mousePos = GetMouseWorldPosition();
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                isDragging = true;
                offset = transform.position - mousePos;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            MoveCircle(GetMouseWorldPosition() + offset);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = GetTouchWorldPosition(touch);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
                if (GetComponent<Collider2D>().OverlapPoint(touchPos))
                {
                    isDragging = true;
                    offset = transform.position - touchPos;
                }
                break;

            case TouchPhase.Moved:
                if (isDragging)
                {
                    MoveCircle(touchPos + offset);
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                break;
        }
    }

    private void MoveCircle(Vector3 newPosition)
    {
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;
    }

    // Updates screen boundaries dynamically based on the camera’s position.
    private void SetScreenBoundaries()
    {
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x + circleRadius;
        maxX = topRight.x - circleRadius;
        minY = bottomLeft.y + circleRadius;
        maxY = topRight.y - circleRadius;
    }

    // Converts mouse position to world position.
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }

    // Converts touch position to world position.
    private Vector3 GetTouchWorldPosition(Touch touch)
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(touch.position);
        pos.z = 0f;
        return pos;
    }
}
