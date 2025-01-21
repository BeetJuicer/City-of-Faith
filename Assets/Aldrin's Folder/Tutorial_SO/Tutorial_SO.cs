using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Scriptable Objects/Tutorial_SO")]
public class Tutorial_SO : ScriptableObject
{
    public string[] tutorialTexts;
    public VideoClip[] tutorialVideos;
}
