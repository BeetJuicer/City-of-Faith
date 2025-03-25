using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] answerButtons;
    public QuizQuestion[] quizQuestions;
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;

    private Color defaultColor = new Color(1f, 1f, 1f, 1f);
    private Color correctColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    private Color wrongColor = new Color(0.9f, 0.2f, 0.2f, 1f);

    public GameObject rewardsPanel;
    public TMP_Text quoteText;
    public TMP_Text goldText;
    public TMP_Text expText;

    private int goldRewardPerCorrect = 10;
    private int expRewardPerCorrect = 5;
    private int gold = 0;
    private int exp = 0;
    private int totalQuestions;

    void Start()
    {
        totalQuestions = quizQuestions.Length;
        LoadQuestion();
        rewardsPanel.SetActive(false);
    }

    // Load the current question and answer choices.
    void LoadQuestion()
    {
        if (currentQuestionIndex >= totalQuestions)
        {
            ShowRewards();
            return;
        }

        QuizQuestion currentQuestion = quizQuestions[currentQuestionIndex];
        questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentQuestion.answers[i];
            answerButtons[i].GetComponent<Image>().color = defaultColor;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
            answerButtons[i].interactable = true;
        }
    }

    // Find the correct answer index, check color green and red, if green add gold and exp.
    void CheckAnswer(int selectedIndex)
    {
        int correctIndex = quizQuestions[currentQuestionIndex].correctAnswerIndex;

        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }

        if (selectedIndex == correctIndex)
        {
            correctAnswers++;
            answerButtons[selectedIndex].GetComponent<Image>().color = correctColor;
            gold += goldRewardPerCorrect;
            exp += expRewardPerCorrect;
        }
        else
        {
            answerButtons[selectedIndex].GetComponent<Image>().color = wrongColor;
            answerButtons[correctIndex].GetComponent<Image>().color = correctColor;
        }

        Invoke("NextQuestion", 2f);
    }

    void NextQuestion()
    {
        currentQuestionIndex++;
        LoadQuestion();
    }

    void ShowRewards()
    {
        rewardsPanel.SetActive(true);
        goldText.text = $"{gold}";
        expText.text = $"{exp}";

        quoteText.text = "Well done! True wisdom comes from continuous learning.";
    }
}
