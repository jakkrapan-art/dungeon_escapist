using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortNumberUI : QuickGameUI
{
    [Header("Animation")]
    [SerializeField] private Animator ChestAnimation;

    public override void setUpUI()
    {
        ChestAnimation.SetBool("isOpen", false);
        for (int i = 0; i < buttons.Count; i++)
        {
            int valueButton = quickGame.GetComponent<SortNumber>().UITextValues.Dequeue();
            setUpUIButton(i, valueButton);
        }
    }

    protected override void setUpUIButton(int buttonIndex, int buttonMethodValue)
    {
        base.setUpUIButton(buttonIndex, buttonMethodValue);
        buttons[buttonIndex].transform.GetChild(0).GetComponent<Text>().text = buttonMethodValue.ToString();
    }
}
