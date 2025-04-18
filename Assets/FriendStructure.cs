using System;
using UnityEngine;

/// <summary>
/// Separate class for friend structures and for viewing purposes only.
/// </summary>
public class FriendStructure : MonoBehaviour
{
    public const string BUILT_VISUAL_STRING = "BuiltVisual";
    public const string INPROGRESS_VISUAL_STRING = "InProgressVisual";
    private GameObject builtVisualGO;
    private GameObject inProgressVisualGO;

    public DateTime TimeBuildfinished { get; private set; }

    private float lastTimeChecked;
    private float checkCooldown = 1f;

    private void Start()
    {
        lastTimeChecked = Time.time;

        builtVisualGO = transform.Find(BUILT_VISUAL_STRING).gameObject;
        inProgressVisualGO = transform.Find(INPROGRESS_VISUAL_STRING).gameObject;

        Debug.Assert(builtVisualGO != null, "BUILT VISUAL NOT FOUND! CHECK SPELLING IF GAMEOBJECT EXISTS.");
        Debug.Assert(inProgressVisualGO != null, "IN PROGRESS VISUAL NOT FOUND! CHECK SPELLING IF GAMEOBJECT EXISTS.");
    }

    public void Init(DateTime timeBuildFinished)
    {
        this.TimeBuildfinished = timeBuildFinished;

        if (timeBuildFinished.CompareTo(DateTime.Now) <= 0)
        {
            //This is built. Enable built visual.
            builtVisualGO.SetActive(true);
            inProgressVisualGO.SetActive(false);
        }
        //else, do nothing. inprogress is active by default.
    }

    private void Update()
    {
        if (builtVisualGO.activeInHierarchy)
        {
            return;
        }

        if (lastTimeChecked + checkCooldown >= Time.time)
            return;

        if (TimeBuildfinished.CompareTo(DateTime.Now) <= 0)
        {
            //This is built. Enable built visual.
            builtVisualGO.SetActive(true);
            inProgressVisualGO.SetActive(false);
        }

    }
}
