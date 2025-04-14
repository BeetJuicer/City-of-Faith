using System;
using System.Collections.Generic;

[Serializable]
public class BibleVerse
{
    public string book_name;
    public int book;
    public int chapter;
    public int verse;
    public string text;
}

[Serializable]
public class BibleWrapper
{
    public List<BibleVerse> verses;
}