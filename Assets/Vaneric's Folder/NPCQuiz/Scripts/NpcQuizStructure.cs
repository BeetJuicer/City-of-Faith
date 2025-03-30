using UnityEngine;

public class NpcQuizStructure : MonoBehaviour
{
    public QuizQuestion_SO quizData;

    public void StartQuiz()
    {
        if (quizData != null)
        {
            FindObjectOfType<QuizManager>().StartQuiz(quizData);
        }
        else
        {
            Debug.LogWarning($"No quiz assigned to {gameObject.name}!");
        }
    }
}
