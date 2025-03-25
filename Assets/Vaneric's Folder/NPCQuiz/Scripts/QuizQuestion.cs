using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz Question", menuName = "Scriptable Objects/Quiz/Question")]
public class QuizQuestion : ScriptableObject
{
    public string question;
    [TextArea] public string[] answers = new string[4];
    public int correctAnswerIndex;
}
