using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "ScriptableObjects/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    public string statusName;
    public Sprite icon;
}
