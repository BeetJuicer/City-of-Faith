using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz Question", menuName = "Scriptable Objects/Quiz/Question")]
public class QuizQuestion_SO : ScriptableObject
{
    public string structureName;
    public GameObject structurePrefab;
    public string npcTitle;
    public string characterName;
    public string question;
    [TextArea] public string[] answers = new string[4];
    public int correctAnswerIndex;
}
