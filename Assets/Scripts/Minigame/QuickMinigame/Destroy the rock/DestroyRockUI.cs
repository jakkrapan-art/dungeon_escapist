using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyRockUI : QuickGameUI
{
    public Transform pickaxeArrow;
    public GameObject breakPointObj;
    public Transform highPoint;
    public Transform lowPoint;
    private float moveSpeed = 180f;
    private bool movingUp = true;
    //public Vector3 posPickaxeArrow;

    public bool isMovingUp
    {
        get{return movingUp; }
        set { movingUp = value; }
    }
    public override void setUpUI()
    {
        setUpUIButton(0, 0);
        pickaxeArrow.localPosition = lowPoint.localPosition;
    }

    public float RandomPosYBreakPoint()
    {
        RectTransform rtBreakObj = (RectTransform)breakPointObj.transform;
        float height = rtBreakObj.rect.height;
        float posY = Random.Range(lowPoint.localPosition.y + (height/2), highPoint.localPosition.y - (height / 2));
        return posY;
    }
    public void UpdatePosYBreakPoint(float posY)
    {
        breakPointObj.transform.localPosition = new Vector3(breakPointObj.transform.localPosition.x, posY);
    }
    public void UpdatePosPickaxeArrow(Vector3 posY)
    {
        pickaxeArrow.transform.localPosition = posY;
    }

    public bool CheakBreakPoint()
    {
        RectTransform rtBreakObj = (RectTransform)breakPointObj.transform;
        float height = rtBreakObj.rect.height;

        float posPickaxe = pickaxeArrow.localPosition.y;
        float posBreak = breakPointObj.transform.localPosition.y;
        return posBreak - (height/2) < posPickaxe && posPickaxe < posBreak + (height / 2);
    }

    
    public bool MoveUp()
    {
        
        pickaxeArrow.localPosition = Vector3.MoveTowards(pickaxeArrow.localPosition, highPoint.localPosition, moveSpeed * Time.fixedDeltaTime);
        quickGame.GetComponent<DestroyRockController>().RPCUpdateMovingPickaxeArrow(pickaxeArrow.localPosition);
        return pickaxeArrow.localPosition.y >= highPoint.localPosition.y;
    }
    public bool MoveDown()
    {
        pickaxeArrow.localPosition = Vector3.MoveTowards(pickaxeArrow.localPosition, lowPoint.localPosition, moveSpeed * Time.fixedDeltaTime);
        quickGame.GetComponent<DestroyRockController>().RPCUpdateMovingPickaxeArrow(pickaxeArrow.localPosition);
        return pickaxeArrow.localPosition.y <= lowPoint.localPosition.y;
       
    }



}
