using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBoardPlayer : MonoBehaviour
{
    public event System.Action playerAction;

    [SerializeField]
    private Transform targetMovePosition;
    public int steps { get; private set; } = 0;

    private Tile currentTile;
    [SerializeField]
    private Tile startTile;
    [SerializeField]
    private BoardTransactionArrow arrowPrefabs;

    private Stack<Tile> pastTiles = new Stack<Tile>();

    public int targetTileIndex;
    private const int DEFAULT_TARGET_TILE_INDEX = 0;

    public bool isPlayerTurn { get; set; }
    public bool isChoosingTransactionTile = false;
    private bool isFirstMove = true;

    private void Start()
    {
        currentTile = startTile;
        //isFirstMove = true;
        targetTileIndex = -1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (steps <= 0)
            {
                rollDice();
            }
            else
            {
                return;
            }

            StartCoroutine(nameof(move));
        }
    }

    private IEnumerator move()
    {
        while (steps > 0)
        {
            StartCoroutine(findTargetPosition(currentTile.AdjacentTiles));

            yield return new WaitUntil(() => !isChoosingTransactionTile);

            //currentTile.playersOnTile.Remove(this);
            yield return new WaitUntil(() => moveToTargetPosition());
            steps -= 1;
            pastTiles.Push(currentTile);
            currentTile = targetMovePosition.GetComponent<Tile>();
            //currentTile.playersOnTile.Add(this);
            targetMovePosition = null;

            //currentTile.doEventForTest(this);

            playerAction?.Invoke();

            if (currentTile.GetComponent<Tile_QuickGame>() != null)
            {
                yield return new WaitUntil(() => QuickGameController.instance.isGameEnded);
            }
            else if (currentTile.GetComponent<Tile_Door>() != null)
            {
                yield return new WaitUntil(() => DoorUIController.instance.IsDone);
            }
        }
    }

    public void rollDice() => steps = Random.Range(1, 7);

    private bool moveToTargetPosition()
    {
        if (transform.position.Equals(targetMovePosition.position))
        {
            return true;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetMovePosition.position, 0.1f);
        return false;
    }

    private IEnumerator findTargetPosition(List<Tile> adjacentTiles)
    {
        List<Tile> tiles = new List<Tile>();
        isChoosingTransactionTile = true;

        if (pastTiles.Count > 0)
        {
            foreach (var tile in adjacentTiles)
            {
                if (!tile.Equals(pastTiles.Peek()))
                {
                    tiles.Add(tile);
                }
            }
        }
        else if (isFirstMove)
        {
            tiles.Add(adjacentTiles[adjacentTiles.Count - 1]);
        }

        if (tiles.Count > 1)
        {
            selectTransactionTile(tiles);
            yield return new WaitUntil(() => targetTileIndex != -1);
            targetMovePosition = tiles[targetTileIndex].transform;
        }
        else
        {
            targetMovePosition = tiles[DEFAULT_TARGET_TILE_INDEX].transform;
        }

        isChoosingTransactionTile = false;
        isFirstMove = false;
        targetTileIndex = -1;
    }

    private void selectTransactionTile(List<Tile> tiles)
    {
        for (int index = 0; index < tiles.Count; index++)
        {
            //Vector3.Lerp ใช้หาระยะทางระหว่าง 2 points ช่องที่3 คือ 0 - 1 อัตราส่วนระยะทาง
            Vector3 instantiatePosition = Vector3.Lerp(transform.position, tiles[index].transform.position, 0.4f);
            var arrow = Instantiate(arrowPrefabs, instantiatePosition, Quaternion.identity);
            arrow.setInfoForTestBoardPlayer(this, tiles[index].transform, index);
        }
    }
}
