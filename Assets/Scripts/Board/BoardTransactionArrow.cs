using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTransactionArrow : MonoBehaviour
{
    [SerializeField]
    private BoardPlayer playerOwner;
    private testBoardPlayer testPlayerOwner;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private int indexTarget = -1;

    public void setArrowInfo(BoardPlayer player, Transform target, int index)
    {
        playerOwner = player;
        this.target = target;
        indexTarget = index;
    }

    public void setInfoForTestBoardPlayer(testBoardPlayer player, Transform target, int index)
    {
        testPlayerOwner = player;
        this.target = target;
        indexTarget = index;
    }

    public int IndexTarget
    {
        get { return indexTarget; }
        set { indexTarget = value; }
    }

    private void OnMouseDown()
    {
        if (playerOwner != null)
            playerOwner.targetTileIndex = indexTarget;
        else if (testPlayerOwner != null)
            testPlayerOwner.targetTileIndex = indexTarget;
    }

    private void Update()
    {
        LockOnTarget();

        if (playerOwner != null)
        {
            if (!playerOwner.isChoosingTransactionTile)
            {
                Destroy(this.gameObject);
            }
        }
        else if (testPlayerOwner != null)
        {
            if (!testPlayerOwner.isChoosingTransactionTile)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(90, (rotation.y - 90)+180, 0f);
    }
}
