using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    public static SelectionIndicator instance;
    [SerializeField]
    private GameObject indicatorObj;

    private void Start()
    {
        instance = this;
    }

    public void SelectionCube(GameObject cubeTarget)
    {
        if (cubeTarget != null)//มีปัญหา
        {
            indicatorObj.SetActive(true);
            this.transform.position = cubeTarget.transform.position;
        }
        else
        {
            indicatorObj.SetActive(false);
        }
    }
}
