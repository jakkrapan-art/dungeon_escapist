using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UserInfo
{
    private string username;
    private string displayName;
    private string email;
    private int wins;
    private int match;
    private int mostScore;

    public string Username
    {
        get { return username; }
    }
    public string Displayname
    {
        get { return displayName; }
        set { displayName = value; }
    }
    public string Email
    {
        get { return email; }
        set { email = value; }
    }

    public int Wins
    {
        get { return wins; }
        set { wins = value; }
    }

    public int Match
    {
        get { return match; }
        set { match = value; }
    }

    public int MostScore
    {
        get { return mostScore; }
        set { mostScore = value; }
    }

    public UserInfo(string displayname, string email)
    {
        this.username = displayname;
        this.displayName = displayname;
        this.email = email;
        this.wins = 0;
        this.match = 0;
        this.mostScore = 0;
    }
   
    
    
}

