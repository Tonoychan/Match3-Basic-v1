using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public int borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePeicePrefabs;

    public float swapTime = 0.3f;

    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePieces;

    Tile m_clickedTile;
    Tile m_targetTile;


    void Start()
    {
        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];
        SetupTiles();
        SetupCamera();
        FillRandom();
    }

    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tile.name = "Tile (" + i + " " + j + ")";
                m_allTiles[i, j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;
                m_allTiles[i, j].init(i, j, this);
            }
        }
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalHeight = (float)height / 2f + (float)borderSize;
        float horizontalHeight = ((float)width / 2f + (float)borderSize) / aspectRatio;
        Camera.main.orthographicSize = (verticalHeight > horizontalHeight) ? verticalHeight : horizontalHeight;
    }

    GameObject GetRandomGamePeice()
    {
        int randomIndex = Random.Range(0, gamePeicePrefabs.Length);

        if (gamePeicePrefabs[randomIndex] == null)
        {
            Debug.LogWarning("Missing Prefab! ");
        }
        return gamePeicePrefabs[randomIndex];
    }

    public void PlaceGamePeice(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("Invalid GamePiece!");
            return;
        }
        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        if (isWithinBounds(x, y))
        {
            m_allGamePieces[x, y] = gamePiece;
        }
        gamePiece.SetCoordinates(x, y);
    }

    bool isWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPeice = Instantiate(GetRandomGamePeice(), Vector3.zero, Quaternion.identity);
                if (randomPeice != null)
                {
                    randomPeice.GetComponent<GamePiece>().init(this);
                    PlaceGamePeice(randomPeice.GetComponent<GamePiece>(), i, j);
                    randomPeice.transform.parent = transform;
                }
            }
        }
    }

    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {            
            m_clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {        
        if (m_clickedTile != null && IsNextTo(tile,m_clickedTile))
        {
            m_targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTile(m_clickedTile, m_targetTile);
        }
        m_clickedTile = null;
        m_targetTile = null;
    }

    void SwitchTile(Tile clickedTile,Tile targetTile)
    {
        GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];

        clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
        targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
    }

    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }
        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }
        return false;
    }

}
