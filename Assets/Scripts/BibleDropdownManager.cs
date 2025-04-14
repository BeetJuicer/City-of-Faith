using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class BibleDropdownManager : MonoBehaviour
{
    public TMP_Dropdown versionDropdown;
    public TMP_Dropdown bookDropdown, chapterDropdown, verseDropdown;
    public TMP_Text verseDisplay;

    public TextAsset kjvJsonFile;
    public TextAsset webJsonFile;
    public TextAsset asvJsonFile;

    private Dictionary<string, List<BibleVerse>> bibleVersions = new();
    private List<BibleVerse> allVerses;
    private string currentBook;
    private int currentChapter;

    void Start()
    {
        // Load and cache each version
        bibleVersions["KJV"] = JsonUtility.FromJson<BibleWrapper>(kjvJsonFile.text).verses;
        bibleVersions["WEB"] = JsonUtility.FromJson<BibleWrapper>(webJsonFile.text).verses;
        bibleVersions["ASV"] = JsonUtility.FromJson<BibleWrapper>(webJsonFile.text).verses;

        // Populate the version dropdown
        versionDropdown.ClearOptions();
        versionDropdown.AddOptions(new List<string> { "KJV", "WEB", "ASV" });
        versionDropdown.onValueChanged.AddListener(OnVersionChanged);

        // Initialize with the first version
        OnVersionChanged(0);
    }

    void OnVersionChanged(int index)
    {
        string selectedVersion = versionDropdown.options[index].text;
        allVerses = bibleVersions[selectedVersion];

        var uniqueBooks = allVerses.Select(v => v.book_name).Distinct().ToList();
        bookDropdown.ClearOptions();
        bookDropdown.AddOptions(uniqueBooks);
        bookDropdown.onValueChanged.RemoveAllListeners();
        bookDropdown.onValueChanged.AddListener(OnBookChanged);

        OnBookChanged(0);
    }

    void OnBookChanged(int index)
    {
        currentBook = bookDropdown.options[index].text;

        var chapters = allVerses
            .Where(v => v.book_name == currentBook)
            .Select(v => v.chapter)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        chapterDropdown.ClearOptions();
        chapterDropdown.AddOptions(chapters.Select(c => c.ToString()).ToList());
        chapterDropdown.onValueChanged.RemoveAllListeners();
        chapterDropdown.onValueChanged.AddListener(OnChapterChanged);

        OnChapterChanged(0);
    }

    void OnChapterChanged(int index)
    {
        currentChapter = int.Parse(chapterDropdown.options[index].text);

        var verses = allVerses
            .Where(v => v.book_name == currentBook && v.chapter == currentChapter)
            .Select(v => v.verse)
            .Distinct()
            .OrderBy(v => v)
            .ToList();

        verseDropdown.ClearOptions();
        verseDropdown.AddOptions(verses.Select(v => v.ToString()).ToList());
        verseDropdown.onValueChanged.RemoveAllListeners();
        verseDropdown.onValueChanged.AddListener(OnVerseChanged);

        OnVerseChanged(0);
    }

    void OnVerseChanged(int index)
    {
        int selectedVerse = int.Parse(verseDropdown.options[index].text);

        var verse = allVerses.FirstOrDefault(v =>
            v.book_name == currentBook &&
            v.chapter == currentChapter &&
            v.verse == selectedVerse
        );

        verseDisplay.text = verse?.text ?? "[Verse not found]";
    }
}

