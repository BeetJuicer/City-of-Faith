using System.Collections;
using UnityEngine;

public class SheepMovements : MonoBehaviour
{
    public float speed = 0.5f;
    private Animator animator;
    private bool isEating = false;
    private GameObject targetFlower;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MoveTowardFlower();
        animator.SetFloat("Blend", isEating ? 1f : 0f);
    }

    private void MoveTowardFlower()
    {
        if (targetFlower == null)
        {
            targetFlower = FindClosestFlower();
        }

        if (targetFlower != null)
        {
            float distance = Vector3.Distance(transform.position, targetFlower.transform.position);

            if (distance > 2f)
            {
                speed = Mathf.Clamp(speed + 0.5f * Time.deltaTime, 0.8f, 1.2f);
            }
            else
            {
                speed = Mathf.Clamp(speed - 0.5f * Time.deltaTime, 0.4f, 0.8f);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetFlower.transform.position, speed * Time.deltaTime);

            if (distance < 0.7f)
            {
                StartCoroutine(EatFlower(targetFlower));
            }

            if (distance > 1.5f && transform.position.x > targetFlower.transform.position.x)
            {
                targetFlower = null;
            }
        }
        else
        {
            speed = 1.0f;
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }

    private GameObject FindClosestFlower()
    {
        GameObject[] flowers = GameObject.FindGameObjectsWithTag("Flower");
        GameObject closestFlower = null;
        float minDistance = Mathf.Infinity;
        float detectionRadius = 5f;

        foreach (GameObject flower in flowers)
        {
            float distance = Vector3.Distance(transform.position, flower.transform.position);

            if (distance < minDistance && distance <= detectionRadius)
            {
                minDistance = distance;
                closestFlower = flower;
            }
        }

        return closestFlower;
    }



    private IEnumerator EatFlower(GameObject flower)
    {
        isEating = true;
        speed = 0f;
        FindObjectOfType<CameraMovement>().enabled = false;
        FindObjectOfType<LoopingBackground>().enabled = false;
        yield return new WaitForSeconds(3f);
        Destroy(flower);
        speed = 0.5f;
        FindObjectOfType<CameraMovement>().enabled = true;
        FindObjectOfType<LoopingBackground>().enabled = true;
        isEating = false;
        targetFlower = null;
    }
}
