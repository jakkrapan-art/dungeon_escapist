using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class testScript : MonoBehaviour
{
    public Text textTouchCount;
    public Text textMouse;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            textTouchCount.text = $"{Input.GetTouch(0).position}";
        }
        textMouse.text = $"{Input.mousePosition}";
    }
}
