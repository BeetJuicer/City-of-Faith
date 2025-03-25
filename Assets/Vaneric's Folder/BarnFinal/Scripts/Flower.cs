using System.Collections;
using UnityEngine;

public class Flower : MonoBehaviour
{
    private bool isBeingEaten = false;
    private Animator sheepAnimator;
    private GameObject sheep;
    private static readonly int Blend = Animator.StringToHash("Blend");

    public float eatingDuration = 5f;

    private CameraMovement cameraMovement;
    private LoopingBackground loopingBackground;
    private GameManagerSheep gameManagerSheep;

    void Awake()
    {
        sheep = GameObject.FindGameObjectWithTag("Sheep");
        if (sheep != null)
        {
            sheepAnimator = sheep.GetComponent<Animator>();
        }

        cameraMovement = FindObjectOfType<CameraMovement>();
        loopingBackground = FindObjectOfType<LoopingBackground>();
        gameManagerSheep = FindObjectOfType<GameManagerSheep>();

        if (gameManagerSheep == null)
            Debug.LogError("❌ GameManagerSheep not found in scene!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Sheep") && !isBeingEaten)
        {
            StartCoroutine(EatFlower());
        }
    }

    private IEnumerator EatFlower()
    {
        isBeingEaten = true;

        if (sheepAnimator != null)
        {
            sheepAnimator.SetFloat(Blend, 1f);
        }

        SetBackgroundState(false);

        yield return new WaitForSeconds(eatingDuration);

        if (sheepAnimator != null)
        {
            sheepAnimator.SetFloat(Blend, 0f);
        }

        SetBackgroundState(true);
        gameManagerSheep?.OnSheepEatsFlower();
        Destroy(gameObject);
    }

    private void SetBackgroundState(bool state)
    {
        if (cameraMovement != null) cameraMovement.enabled = state;
        if (loopingBackground != null) loopingBackground.enabled = state;
    }
}
