using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Hash
{
    public static string hash(string data)
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(data);
        SHA256Managed mySha256 = new SHA256Managed();

        byte[] hashValues = mySha256.ComputeHash(textToBytes);
        return getHaxStringFromHash(hashValues);
    }

    private static string getHaxStringFromHash(byte[] hash)
    {
        string hexString = "";
        foreach (var item in hash)
        {
            hexString += item.ToString("x2");
        }

        return hexString;
    }
}
