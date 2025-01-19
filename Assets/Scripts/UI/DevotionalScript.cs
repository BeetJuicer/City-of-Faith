using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System;

public class DevotionalScript : MonoBehaviour
{
    public GameObject DevotionalUI;
    public GameObject DevotionalNextButton;
    public TMP_Text VerseReferenceText; // TMP object for the verse reference
    public TMP_Text VerseContentText;   // TMP object for the verse content
    [SerializeField] public GameObject HUDCanvas;
    [SerializeField] private CentralHall centralHall;


    private List<Verse> verses;

    void Start()
    {

        ClearPreviousDate();

        // Load the JSON file and parse it into the list of verses
        LoadVerses();

        // Get the current system date
        DateTime currentDate = DateTime.Now.Date;

        Debug.Log("Current Date: " + currentDate.ToString("yyyy-MM-dd"));

        // Check if it's a new day
        if (IsNewDay(currentDate) && centralHall.Level != 1)
        {
            // Debug log to confirm the condition passed
            Debug.Log("New day detected and player has seen the cutscene.");

            // Get a random verse and display it
            Verse verse = GetRandomVerse();
            if (verse != null)
            {
                Debug.Log($"Today's Verse: {verse.reference} - {verse.content}");

                // Set the text of the TMP game objects
                VerseReferenceText.text = verse.reference;
                VerseContentText.text = verse.content;
            }

            HUDCanvas.SetActive(false);
            OpenDevotional();
            SaveCurrentDate(currentDate);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Optional: You can add code here to continuously monitor for a new day, if required
    }

    public void OpenDevotional()
    {
        DevotionalUI.SetActive(true);
        DevotionalNextButton.SetActive(false);
        Debug.Log("Devotional UI opened.");
    }

    private bool IsNewDay(DateTime currentDate)
    {
        string previousDateStr = PlayerPrefs.GetString("PreviousDate", "");

        if (string.IsNullOrEmpty(previousDateStr))
        {
            return true;
        }

        DateTime previousDate = DateTime.Parse(previousDateStr);
        return currentDate > previousDate;
    }

    private void SaveCurrentDate(DateTime currentDate)
    {
        PlayerPrefs.SetString("PreviousDate", currentDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
        Debug.Log("Date saved: " + currentDate.ToString("yyyy-MM-dd"));
    }

    public void ClearPreviousDate()
    {
        PlayerPrefs.DeleteKey("PreviousDate");
        PlayerPrefs.DeleteKey("UsedIndexes");
        PlayerPrefs.Save();
        Debug.Log("Previous date cleared from PlayerPrefs.");
    }

    private void LoadVerses()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("bible_verses_net");
        if (jsonText != null)
        {
            VerseList verseList = JsonUtility.FromJson<VerseList>(jsonText.text);
            verses = new List<Verse>(verseList.verses);
        }
        else
        {
            Debug.LogError("JSON file not found.");
        }
    }

    private Verse GetRandomVerse()
    {
        string usedIndexesStr = PlayerPrefs.GetString("UsedIndexes", "");
        List<int> usedIndexes = new List<int>();
        if (!string.IsNullOrEmpty(usedIndexesStr))
        {
            usedIndexes = new List<int>(Array.ConvertAll(usedIndexesStr.Split(','), int.Parse));
        }

        List<Verse> unusedVerses = new List<Verse>();
        for (int i = 0; i < verses.Count; i++)
        {
            if (!usedIndexes.Contains(i))
            {
                unusedVerses.Add(verses[i]);
            }
        }

        if (unusedVerses.Count == 0)
        {
            Debug.Log("All verses have been used.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, unusedVerses.Count);
        Verse randomVerse = unusedVerses[randomIndex];

        int verseIndex = verses.IndexOf(randomVerse);
        usedIndexes.Add(verseIndex);

        PlayerPrefs.SetString("UsedIndexes", string.Join(",", usedIndexes));
        PlayerPrefs.Save();

        return randomVerse;
    }
}

[Serializable]
public class Verse
{
    public int index;
    public string reference;
    public string content;
}

[Serializable]
public class VerseList
{
    public List<Verse> verses;
}
