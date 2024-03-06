using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Tilemap tileMap;

    [SerializeField] public GamePiece[] gamePieces;

    public List<Selectable> allPieces = new List<Selectable>();

    public List<Vector3Int> boardCurrent = new List<Vector3Int>();
    public List<Vector3Int> boardOneTurn = new List<Vector3Int>();
    public List<Vector3Int> boardTwoTurn = new List<Vector3Int>();
    public List<Vector3Int> boardThreeTurn = new List<Vector3Int>();
    public List<Vector3Int> boardFourTurn = new List<Vector3Int>();

    public int currentTurnForRep = 0;

    public int nRep = 0;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

	private void Start()
	{
        InstanceBoard();
    }

    public void InstanceBoard()
    {
        ClearBoard();
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GamePiece gp = gamePieces[j];

                GameObject go = Instantiate(gp.piecePrefab);
                Selectable selectable = go.GetComponent<Selectable>();
                if (i == 0)
                {
                    go.transform.tag = "Player01";
                    go.transform.position = tileMap.GetCellCenterWorld(gp.pieceCoord);
                    //Board.Instance.currentBoard[gp.pieceCoord.y].currentRowBoard[gp.pieceCoord.x] = 1;
                    Board.Instance.tileMap.SetTileFlags(gp.pieceCoord, TileFlags.None);
                    Board.Instance.getCustomTile(gp.pieceCoord).cardOnTile = selectable;
                    selectable.cellPos = gp.pieceCoord;
                }
                else
                {
                    go.transform.tag = "Player02";
                    go.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(2, 3, 0) - gp.pieceCoord);
                    //Board.Instance.currentBoard[3 - gp.pieceCoord.y].currentRowBoard[2 - gp.pieceCoord.x] = 2;
                    Board.Instance.tileMap.SetTileFlags(gp.pieceCoord, TileFlags.None);
                    Board.Instance.getCustomTile(new Vector3Int(2, 3, 0) - gp.pieceCoord).cardOnTile = selectable;
                    go.transform.eulerAngles = new Vector3(0, 0, 180);
                    selectable.cellPos = new Vector3Int(2, 3, 0) - gp.pieceCoord;
                }
                allPieces.Add(selectable);
            }
        }
    }

    public void killUnit(Selectable s)
    {
        allPieces.Remove(s);
        Destroy(s.gameObject);
    }

    private void ClearBoard()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector3Int pos = new Vector3Int(j, i, 0);
                if (Board.Instance.getCustomTile(pos) != null)
                {
                    Board.Instance.changeTileColor(pos, Color.white);
                    Board.Instance.getCustomTile(pos).clickAction.RemoveAllListeners();
                    Board.Instance.getCustomTile(pos).cardOnTile = null;
                }
            }
        }

        for (int i = allPieces.Count - 1; i >= 0; i--)
        {
            Destroy(allPieces[i].gameObject);
        }

        allPieces.Clear();
    }

    private List<Vector3Int> getBoard(List<Selectable> pieces)
    {
        List<Vector3Int> board = new List<Vector3Int>();
        foreach (Selectable s in pieces)
        {
            board.Add(s.cellPos);
        }
        return board;
    }

    public void SaveBoards()
    {
        currentTurnForRep++;
        boardCurrent = getBoard(allPieces);
        if (currentTurnForRep > 3)
        {
            boardFourTurn = boardThreeTurn;
        }
        if (currentTurnForRep > 2)
        {
            boardThreeTurn = boardTwoTurn;
        }
        if (currentTurnForRep > 1)
        {
            boardTwoTurn = boardOneTurn;
        }
        boardOneTurn = boardCurrent;
    }

    public void CheckRepetition()
    {
        if (currentTurnForRep >= 4)
        {
            boardCurrent = getBoard(allPieces);
            if (boardFourTurn.SequenceEqual(boardCurrent))
            {
                currentTurnForRep = 0;
                nRep++;
                if (nRep >= 3)
                {
                    UIManager.Instance.StaleMate();
                }
            }
            else
            {
                nRep = 0;
            }
        }
    }
}

[System.Serializable]
public class GamePiece
{
    public GameObject piecePrefab;
    public Vector3Int pieceCoord;
}
