using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LocalSave
{
    public class LocalSave : MonoBehaviour
    {

        public void SaveUser(string userJson)
        {
            PlayerPrefs.SetString("user", userJson);
        }



        public void SaveQuestion(List<QuestionData> questionData)
        {
            string json = JsonConvert.SerializeObject(questionData);
            PlayerPrefs.SetString("grammarQuestion", json);
        }

        public List<QuestionData> LoadQuestion()
        {
            List<QuestionData> questionData = JsonConvert.DeserializeObject<List<QuestionData>>(PlayerPrefs.GetString("grammarQuestion"));
            return questionData;
        }

        public void SaveHistory(string historyJson)
        {
            PlayerPrefs.SetString("History", historyJson);
        }
    }
}