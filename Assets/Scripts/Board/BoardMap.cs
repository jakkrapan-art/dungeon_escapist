using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMap : MonoBehaviour
{
    [SerializeField] private Tile startTile;

    #region getter_and_setter
    public Tile StartTile
    {
        get { return startTile; }
    }
    #endregion
}
