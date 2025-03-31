using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/NPC_SO")]
public class NPC_SO : ScriptableObject
{
    [Header("NPC Settings")]
    [Tooltip("Radius within which buildings can be detected and affected.")]
    public float detectionRadius = 5f;

    [Tooltip("NPC asset (e.g., Prefab, Model, or Sprite).")]
    public string npcAsset;

    [Header("Buildings That Can Be Boosted")]
    [Tooltip("List of buildings that can be detected and boosted within the radius.")]
    public List<string> detectableBuildings = new List<string>();

}
