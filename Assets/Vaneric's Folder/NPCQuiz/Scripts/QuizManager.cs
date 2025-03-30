using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text npcTitleText;
    public TMP_Text characterNameText;
    public Button[] answerButtons;
    public GameObject quizPanel;
    private QuizQuestion_SO currentQuiz;
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

    void Start()
    {
        quizPanel.SetActive(false);
        rewardsPanel.SetActive(false);
    }

    public void StartQuiz(QuizQuestion_SO quizData)
    {
        currentQuiz = quizData;
        quizPanel.SetActive(true);
        correctAnswers = 0;
        LoadQuestion();
    }

    void LoadQuestion()
    {
        if (currentQuiz == null)
        {
            Debug.LogWarning("No quiz loaded!");
            return;
        }

        npcTitleText.text = currentQuiz.npcTitle;
        characterNameText.text = currentQuiz.characterName;
        questionText.text = currentQuiz.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentQuiz.answers[i];
            answerButtons[i].GetComponent<Image>().color = defaultColor;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
            answerButtons[i].interactable = true;
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        int correctIndex = currentQuiz.correctAnswerIndex;

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

        Invoke("ShowRewards", 2f);
    }

    void ShowRewards()
    {
        rewardsPanel.SetActive(true);
        goldText.text = $"{gold}";
        expText.text = $"{exp}";
        quoteText.text = "Well done! True wisdom comes from continuous learning.";
    }
}
