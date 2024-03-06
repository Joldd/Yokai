using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;

public class GameManager : MonoBehaviour, IGameManager
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Tilemap tileMap;

    [SerializeField] public GamePiece[] gamePieces;

    [SerializeField] public Transform player01, player02;

    public List<Selectable> allPieces = new List<Selectable>();

    private List<Vector2Int> boardCurrent = new List<Vector2Int>();
    private List<Vector2Int> boardOneTurn = new List<Vector2Int>();
    private List<Vector2Int> boardTwoTurn = new List<Vector2Int>();
    private List<Vector2Int> boardThreeTurn = new List<Vector2Int>();
    private List<Vector2Int> boardFourTurn = new List<Vector2Int>();

    private int currentTurnForRep = 0;

    private int nRep = 0;

    public bool gamePaused = true;

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
                    go.transform.SetParent(player01);
                    go.transform.tag = go.transform.parent.tag;
                    go.transform.position = tileMap.GetCellCenterWorld((Vector3Int)gp.pieceCoord);

                    //Board.Instance.currentBoard[gp.pieceCoord.y].currentRowBoard[gp.pieceCoord.x] = 1;
                    Board.Instance.tileMap.SetTileFlags((Vector3Int)gp.pieceCoord, TileFlags.None);
                    Board.Instance.getCustomTile(gp.pieceCoord).cardOnTile = selectable;
                    selectable.cellPos = gp.pieceCoord;
                }
                else
                {
                    go.transform.SetParent(player02);
                    go.transform.tag = go.transform.parent.tag;
                    go.transform.position = tileMap.GetCellCenterWorld((Vector3Int)(new Vector2Int(2, 3) - gp.pieceCoord));

                    //Board.Instance.currentBoard[3 - gp.pieceCoord.y].currentRowBoard[2 - gp.pieceCoord.x] = 2;
                    Board.Instance.tileMap.SetTileFlags((Vector3Int)gp.pieceCoord, TileFlags.None);
                    Board.Instance.getCustomTile(new Vector2Int(2, 3) - gp.pieceCoord).cardOnTile = selectable;
                    go.transform.eulerAngles = new Vector3(0, 0, 180);
                    selectable.cellPos = new Vector2Int(2, 3) - gp.pieceCoord;
                }
                allPieces.Add(selectable);
            }
        }
    }

    public void killUnit(Selectable s)
    {
        //allPieces.Remove(s);
        //Destroy(s.gameObject);
        s.isDead = true;
        //s.gameObject.SetActive(false);
        s.gameObject.transform.position = new Vector3(100, 100, 0);
    }

    private void ClearBoard()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pos = new Vector2Int(j, i);
                if (Board.Instance.getCustomTile(pos) != null)
                {
                    Board.Instance.changeTileColor(pos, Color.white);
                    Board.Instance.getCustomTile(pos).clickAction.RemoveAllListeners();
                    Board.Instance.getCustomTile(pos).cardOnTile = null;
                    Board.Instance.getCustomTile(pos).tilePosition = new Vector2Int(j, i);
                }
            }
        }

        for (int i = allPieces.Count - 1; i >= 0; i--)
        {
            Destroy(allPieces[i].gameObject);
        }

        allPieces.Clear();
    }

    private List<Vector2Int> getBoard(List<Selectable> pieces)
    {
        List<Vector2Int> board = new List<Vector2Int>();
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

    //public void CheckMat()
    //{
    //    bool isMat = true;
    //    foreach (Selectable s in allPieces)
    //    {
    //        if(s.TryGetComponent<Koropokkuru>(out Koropokkuru koro)){
    //            if(koro.tag == "Player01" && koro.cellPos.y == 3)
    //            {
    //                foreach (Selectable s1 in allPieces)
    //                {
    //                    if (s1.tag == "Player02")
    //                    {
    //                        foreach (Vector2Int move in s1.movablePos)
    //                        {
    //                            if (move == s.cellPos)
    //                            {
    //                                isMat = false;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            else if (koro.tag == "Player02" && koro.cellPos.y == 0)
    //            {

    //            }
    //        }
    //    }
    //}

    //Interface IGameManager
	public List<IPawn> GetAllPawn()
	{
        List<IPawn> pawnList = new List<IPawn>();
		foreach (Selectable pawn in allPieces)
		{
            pawnList.Add(pawn);
		}
        return pawnList;
	}

	public List<IBoardCase> GetAllBoardCase()
	{
        List<IBoardCase> boardCaseList = new List<IBoardCase>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pos = new Vector2Int(j, i);

                CustomTile ct = Board.Instance.getCustomTile(pos);
                if (ct != null)
                    boardCaseList.Add(ct);
            }
        }

        return boardCaseList;
	}

	public void DoAction(IPawn pawnTarget, Vector2Int position, EActionType actionType)
	{
		switch (actionType)
        {
            case (EActionType.MOVE):
                
                break;

            case (EActionType.PARACHUTE):

                break;
            default:
                break;
        }
	}
}

[System.Serializable]
public class GamePiece
{
    public GameObject piecePrefab;
    public Vector2Int pieceCoord;
}
