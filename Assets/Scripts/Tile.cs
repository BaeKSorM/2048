using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public RaycastHit2D[] hits;
    public RectTransform rectTransform;
    public RaycastHit2D hit;
    public BoxCollider2D BC;
    public Rigidbody2D RB;
    public float squareSize;
    public List<TMP_Text> moveDirectionTileTexts;
    public int moveDirectionTileCount;
    public string tileText;
    public int sameTileCount;
    public bool moveEnd;
    public int sameTileCheck;
    public Vector2 movePos;
    void Awake()
    {
        BC = GetComponent<BoxCollider2D>();
        RB = GetComponent<Rigidbody2D>();
        rectTransform = transform.GetComponent<RectTransform>();
    }
    void Start()
    {
        squareSize = BC.size.x;
        canMove = true;
    }
    public bool canMove;
    void OnEnable()
    {
        BC.isTrigger = false;
        tileText = GetComponentInChildren<TMP_Text>().text;
        moveDirectionTileTexts = new List<TMP_Text>();
        moveDirectionTileCount = 1;
        sameTileCount = 0;
        sameTileCheck = 0;
        // if (canMove)
        // {
        GameManager.Instance.tiles.Add(rectTransform);
        GameManager.Instance.tileRBs.Add(RB);
        ++GameManager.Instance.currentTiles;
        // }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("TileChecker"))
        {
            GameManager.Instance.activeTiles[int.Parse(other.name)] = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TileChecker"))
        {
            GameManager.Instance.activeTiles[int.Parse(other.name)] = false;
        }
    }
    public void Move(Vector2 _movePos)
    {
        StartCoroutine(MoveCoroutine(_movePos));
    }
    IEnumerator MoveCoroutine(Vector2 _movePos)
    {
        Debug.Log(2);
        movePos = _movePos;
        tileText = GetComponentInChildren<TMP_Text>().text;
        moveDirectionTileTexts = new List<TMP_Text>();
        moveDirectionTileCount = 1;
        sameTileCount = 0;
        sameTileCheck = 0;
        moveEnd = false;
        Vector3 startRectPos = _movePos + rectTransform.anchoredPosition;
        hits = Physics2D.RaycastAll((Vector2)transform.position + movePos * squareSize, movePos, Mathf.Infinity, GameManager.Instance.layer);
        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].transform.CompareTag("Tile"))
            {
                ++moveDirectionTileCount;
                moveDirectionTileTexts.Add(hits[i].transform.GetComponentInChildren<TMP_Text>());
            }
            else if (hits[i].transform.CompareTag("Block"))
            {
                hit = hits[i];
            }
        }
        for (int i = 0; i < moveDirectionTileTexts.Count; ++i)
        {
            if (moveDirectionTileTexts[i].text != tileText)
            {
                break;
            }
            ++sameTileCount;
        }
        if (sameTileCount % 2 == 1)
        {
            --moveDirectionTileCount;
            BC.isTrigger = true;
            sameTileCheck = 1;
            StartCoroutine(TileValueUp());
        }
        if (hits.Length > 1)
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].transform.CompareTag("Tile") && hits[i].transform.GetComponent<Tile>().sameTileCount > sameTileCheck)
                {
                    moveDirectionTileCount -= 1;
                }
            }
        }
        if (hit)
        {
            Vector2 pos = (Vector2)hit.transform.position - _movePos * squareSize * moveDirectionTileCount;
            if (transform.position == (Vector3)pos)
            {
                Debug.Log($"{gameObject.name} : Not Moved");
                ++GameManager.Instance.notMovedTiles;
            }
            if (startRectPos.x != GameManager.Instance.leftEndPos &&
              startRectPos.x != GameManager.Instance.rightEndPos &&
              startRectPos.y != GameManager.Instance.upEndPos &&
              startRectPos.y != GameManager.Instance.downEndPos)
            {
                while (transform.position != (Vector3)pos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, (Vector3)pos, GameManager.Instance.tileMoveSpeed);
                    yield return null;
                }
                transform.position = (Vector3)pos;
            }
        }
        moveEnd = true;
        ++GameManager.Instance.arrivedTiles;
        movePos = Vector2.zero;
    }
    IEnumerator TileValueUp()
    {
        yield return new WaitUntil(() => moveEnd);
        GameManager.Instance.tiles.Remove(GetComponent<RectTransform>());
        GameManager.Instance.tileRBs.Remove(RB);
        int score = int.Parse(tileText) * 2;
        moveDirectionTileTexts[0].text = score.ToString();
        UIManager.Instance.ScoreUp(score);
        --GameManager.Instance.currentTiles;
        --GameManager.Instance.arrivedTiles;
        gameObject.SetActive(false);
    }
}