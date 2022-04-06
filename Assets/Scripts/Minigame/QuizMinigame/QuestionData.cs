using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public int id { get; private set; }
    public string question { get; private set; }
    public QuestionCategory category { get; private set; }
    public string correctAns { get;  private set; }
    public string wrongAns1 { get; private set; }
    public string wrongAns2 { get;  private set; }
    public string wrongAns3 { get; private set; }
    public string wrongAns4 { get;  private set; }
    public string wrongAns5 { get;  private set; }
    public string wrongAns6 { get; private set; }
    public string ansExplain { get;  private set; }

    [JsonConstructor]
    public QuestionData(int id, string question, string category, string correctAns, string wrongAns1, string wrongAns2, string wrongAns3, string wrongAns4, string wrongAns5, string wrongAns6)
    {
        this.id = id;
        this.question = question;
        this.category = category == "grammar" ? QuestionCategory.Grammar : QuestionCategory.Tense;
        this.correctAns = correctAns;
        this.wrongAns1 = wrongAns1;
        this.wrongAns2 = wrongAns2;
        this.wrongAns3 = wrongAns3;
        this.wrongAns4 = wrongAns4;
        this.wrongAns5 = wrongAns5;
        this.wrongAns6 = wrongAns6;
    }

    public override string ToString()
    {
        return $"Question({question},{category.ToString()},{correctAns},{wrongAns1},{wrongAns2},{wrongAns3},{wrongAns4},{wrongAns5},{wrongAns6},{ansExplain})";
    }
}

public enum QuestionCategory { Tense, Grammar }
