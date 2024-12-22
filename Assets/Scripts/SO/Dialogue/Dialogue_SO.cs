using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue Lists")]
public class Dialogue_SO : ScriptableObject
{
    public string[] dialogueLines;
    public Sprite npcImage;
}
