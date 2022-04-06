using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionDatabase : MonoBehaviour
{
    public List<QuestionData> questions { get; private set; } = new List<QuestionData>();
}
