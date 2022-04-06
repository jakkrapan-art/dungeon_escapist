using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class User
{
    [JsonProperty("username")]
    private string username;
    [JsonProperty("displayName")]
    private string displayName;
    [JsonProperty("email")]
    private string email;
    [JsonProperty("matchCount")]
    private int matchCount;
    [JsonProperty("winCount")]
    private int winCount;
    [JsonProperty("mostScore")]
    private int mostScore;
    public string getUsername() => username;
    public string getEmail() => email;
    public string getDisplayName() => displayName;

    public string getDisplayName(int length)
    {
        {
            if (displayName.Length <= length)
            {
                return displayName;
            }

            string name = displayName.Substring(0, length - 5) + "...";

            return name;
        }
    }
    public int getMatchCount() => matchCount;
    public int getWinCount() => winCount;
    public int getMostScore() => mostScore;

    [JsonConstructor]
    public User(string Username, string DisplayName, string Email, int Match, int MostScore, int Win)
    {
        username = Username;
        displayName = DisplayName;
        email = Email;
        matchCount = Match;
        winCount = Win;
        mostScore = MostScore;
    }

    public override string ToString()
    {
        return $"User[Username: {username}, DisplayName: {displayName}, Email: {email}, MatchCount: {matchCount}, WinCount: {winCount}, MostScore: {mostScore}]";
    }
}
