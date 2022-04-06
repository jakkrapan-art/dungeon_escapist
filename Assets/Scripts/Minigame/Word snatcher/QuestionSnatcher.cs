using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class QuestionSnatcher
{
    public string hint { get; private set; }
    private string word;

    public string getWordText()
    {
        return word;
    }

    public char[] getWord()
    {
        return word.ToCharArray();
    }
    public string getHint()
    {
        return hint;
    }

    [JsonConstructor]
    public QuestionSnatcher(string hint, string questionAns)
    {
        this.hint = hint;
        this.word = questionAns;
    }

    public override string ToString()
    {
        return "word = " + word.ToString();
    }

}