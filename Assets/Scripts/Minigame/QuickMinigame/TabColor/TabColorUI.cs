using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TabColorUI : QuickGameUI
{
    [Header("Hint GameObject")]
    public List<GameObject> HintColorSprites;
    public List<Transform> HintSpritePosition;
    public GameObject HintHeader;
    public List<Image> lightImages;

    public List<Sprite> symbols;
    public Transform HintImagesParent;

    protected override void setUpUIButton(int buttonIndex, int buttonMethodValue)
    {
        base.setUpUIButton(buttonIndex, buttonMethodValue);
    }

    public void ResetLamp()
    {
        Debug.Log("Start ResetLamp");
        foreach (var l in lightImages)
        {
            Debug.Log("Lamp 1");
            l.color = Color.white;
        }
    }
    protected override void resetUI()
    {
        base.resetUI();
        playTimeRemainBar.fillAmount = 1;
        playTimeRemainText.text = "0";
    }


    public void setLight(int index, Color lightColor)
    {
        lightImages[index].color = lightColor;
    }

    public void showHintPopup()
    {
        HintHeader.SetActive(true);
    }
    public void hideHintPopup()
    {
        HintHeader.SetActive(false);
    }
    public void setUpHintSymbol(Queue<int> symbolIndexes)
    {
        Debug.Log("Indexes: ");
        for (int i = 0; i < HintImagesParent.childCount; i++)
        {
            Debug.Log(symbolIndexes.Peek());
            Image image = HintImagesParent.GetChild(i).GetComponent<Image>();
            Sprite sprite = symbols[symbolIndexes.Dequeue()];
            image.sprite = sprite;
        }
    }
}
