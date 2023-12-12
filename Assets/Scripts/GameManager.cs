using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static GameManager Instance;
    public Vector2 startPos;
    public Vector2 endPos;
    public List<RectTransform> tiles;
    public List<Rigidbody2D> tileRBs;
    public float tileMoveSpeed;
    public Transform tilesTrans;
    public bool canMove;
    public int leftEndPos;
    public int rightEndPos;
    public int upEndPos;
    public int downEndPos;
    public int arrivedTiles;
    public int notMovedTiles;
    public LayerMask layer;
    public int currentTiles;
    public List<bool> activeTiles;
    public List<Vector2> tilesPoses;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        tilesTrans = GameObject.Find("Tiles").transform;
        int firstTile = Random.Range(0, tilesTrans.childCount);
        int secondTile;
        do
        {
            secondTile = Random.Range(0, tilesTrans.childCount); ;
        } while (firstTile == secondTile);
        tilesTrans.GetChild(firstTile).gameObject.SetActive(true);
        tilesTrans.GetChild(secondTile).gameObject.SetActive(true);
        // for (int i = 0; i < tilesTrans.childCount; ++i)
        // {
        //     if (tilesTrans.GetChild(i).gameObject.activeSelf)
        //     {
        //         tiles.Add(tilesTrans.GetChild(i).GetComponent<RectTransform>());
        //         tileRBs.Add(tiles[tiles.Count - 1].GetComponent<Rigidbody2D>());
        //     }
        // }
    }
    void Update()
    {
        if (canMove)
        {
            SlideDelay();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (arrivedTiles == 0)
        {
            startPos = eventData.position;
            canMove = true;
        }
    }
    public float delayMax;
    public float delayTime;
    void SlideDelay()
    {
        delayTime += Time.deltaTime;
        if (delayTime > delayMax)
        {
            canMove = false;
            delayTime = 0;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (arrivedTiles == 0)
        {
            endPos = eventData.position;
            if (canMove)
            {
                MoveDirection(endPos - startPos);
                canMove = false;
                delayTime = 0;
            }
        }
    }
    public enum MoveType { right, left, up, down };
    public MoveType moveType;
    void MoveDirection(Vector2 power)
    {
        Vector2 movePos = Vector2.zero;
        if (Mathf.Abs(power.x) > 10 && Mathf.Abs(power.y) > 10)
        {
            if (Mathf.Abs(power.x) > Mathf.Abs(power.y))
            {
                if (power.x > 0)
                {
                    movePos = Vector2.right;
                    moveType = MoveType.right;
                }
                else if (power.x < 0)
                {
                    movePos = Vector2.left;
                    moveType = MoveType.left;
                }
            }
            else
            {
                if (power.y > 0)
                {
                    movePos = Vector2.up;
                    moveType = MoveType.up;
                }
                else if (power.y < 0)
                {
                    movePos = Vector2.down;
                    moveType = MoveType.down;
                }
            }
            TileMove(movePos);
        }
    }
    void TileMove(Vector2 movePos)
    {
        arrivedTiles = 0;
        switch (moveType)
        {
            case MoveType.left:
                TilesMoveRightToLeft(movePos);
                break;
            case MoveType.right:
                TilesMoveLeftToRight(movePos);
                break;
            case MoveType.up:
                TilesMoveDownToUp(movePos);
                break;
            case MoveType.down:
                TilesMoveUpToDown(movePos);
                break;
        }
        if (notMovedTiles != currentTiles)
        {
            StartCoroutine(AddTile());
        }
        else
        {
            notMovedTiles = 0;
            arrivedTiles = 0;
        }
    }
    IEnumerator AddTile()
    {
        yield return new WaitUntil(() => arrivedTiles == currentTiles);
        yield return new WaitForSeconds(.5f);
        // Debug.Log(tilesTrans.childCount);
        // Debug.Log(activeTiles.Count);
        for (int i = 0; i < tilesTrans.childCount; ++i)
        {
            Debug.Log(0);
            if (!tilesTrans.GetChild(i).gameObject.activeSelf)
            {
                while (true)
                {
                    int blankTile = Random.Range(0, activeTiles.Count);
                    Debug.Log(blankTile);
                    Debug.Log(activeTiles[blankTile]);
                    if (!activeTiles[blankTile])
                    {
                        tilesTrans.GetChild(i).gameObject.SetActive(true);
                        tilesTrans.GetChild(i).GetComponentInChildren<TMP_Text>().text = "2";
                        tilesTrans.GetChild(i).GetComponent<RectTransform>().anchoredPosition = tilesPoses[blankTile];
                        activeTiles[blankTile] = true;
                        // yield return new WaitForSeconds(2);
                        notMovedTiles = 0;
                        arrivedTiles = 0;
                        yield break;
                    }
                    yield return null;
                }
            }
            yield return null;
        }
        Debug.Log("NOT REMAIN TILE");
        UIManager.Instance.endingPanel.SetActive(true);
    }
    void TilesMoveLeftToRight(Vector2 movePos)
    {
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == 300)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == 100)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == -100)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == -300)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
    }
    void TilesMoveRightToLeft(Vector2 movePos)
    {
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == -300)
            {
                Debug.Log(11);

                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == -100)
            {
                Debug.Log(11);
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == 100)
            {
                Debug.Log(11);
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.x == 300)
            {
                Debug.Log(11);
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
    }
    void TilesMoveDownToUp(Vector2 movePos)
    {
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == 300)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == 100)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == -100)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == -300)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
    }
    void TilesMoveUpToDown(Vector2 movePos)
    {
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == -300)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == -100)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == 100)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (tiles[i].anchoredPosition.y == 300)
            {
                tiles[i].GetComponent<Tile>().Move(movePos);
            }
        }
    }
}