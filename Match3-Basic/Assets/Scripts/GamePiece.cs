using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    Board m_board;

    bool m_isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetCoordinates(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void init(Board board)
    {
        m_board = board;
    }

    public void Move(int destX, int destY, float moveSpeed)
    {
        if (m_isMoving)
            return;
        StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), moveSpeed));
    }
    IEnumerator MoveRoutine(Vector3 destination, float moveSpeed)
    {
        Vector3 startPosition = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0f;
        m_isMoving = true;
        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                if (m_board != null)
                {
                    m_board.PlaceGamePeice(this, (int)destination.x, (int)destination.y);
                }
                break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime/moveSpeed,0f,1f);

            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }
        m_isMoving = false;
    }
}
