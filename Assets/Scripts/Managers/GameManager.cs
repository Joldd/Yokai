using Groupe10;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;
using YokaiNoMori.Struct;

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

    public bool isIA;

    public bool isGameOver;

    [SerializeField] public Transform[] capturedPanel;

    public MinimaxAlgorithm minimaxAlgorithm;

    public int turn;

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
        minimaxAlgorithm = GetComponent<MinimaxAlgorithm>();
        turn = 1;
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
        UpdateAllMovablePos();
    }

    public void killUnit(Selectable s)
    {
        //allPieces.Remove(s);
        //Destroy(s.gameObject);
        s.isDead = true;
        //s.gameObject.SetActive(false);
        s.transform.position = new Vector3(100, 100, 0);
        s.cellPos = Board.Instance.getTilePos(s.transform.position);
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

    public Selectable GetPawn(Vector2Int pos)
    {
        foreach (Selectable s in allPieces)
        {
            if (s.cellPos == pos)
            {
                return s;
            }
        }
        return null;
    }

    public CapturedCard GetCapturedCard(TempPawn pawn)
    {

        for (int i = 0; i < capturedPanel[1].childCount; i++)
        {
            if (capturedPanel[1].GetChild(i).gameObject.TryGetComponent<CapturedCard>(out CapturedCard capturedCard))
            {
                if (capturedCard.currentCard.cardType == pawn.pawnType && capturedCard.player == 2 && !pawn.isEnemy)
                {
                    return capturedCard;
                }
            }
        }
        return null;
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

    public void UpdateAllMovablePos()
    {
        foreach (Selectable s in allPieces)
        {
            s.addAllMovable();
        }
    }

    public bool CheckMat(Transform winner, Transform looser)
    {
        bool isMat = false;

        for (int i = 0; i < winner.childCount; i++)
        {
            Selectable si = winner.GetChild(i).GetComponent<Selectable>();
            if (si.TryGetComponent<Koropokkuru>(out Koropokkuru koro))
            {
                if (koro.tag == "Player01" && koro.cellPos.y == 3)
                {
                    isMat = true;
                    for (int j = 0; j < looser.childCount; j++)
                    {
                        if (!isMat) break;

                        Selectable sj = looser.GetChild(j).GetComponent<Selectable>();
                        if (!sj.isDead)
                        {
                            foreach (Vector2Int move in sj.movablePos)
                            {
                                if (koro.cellPos == move)
                                {
                                    isMat = false;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                else if (koro.tag == "Player02" && koro.cellPos.y == 0)
                {
                    isMat = true;
                    for (int j = 0; j < looser.childCount; j++)
                    {
                        if (!isMat) break;

                        Selectable sj = looser.GetChild(j).GetComponent<Selectable>();
                        if (!sj.isDead)
                        {
                            foreach (Vector2Int move in sj.movablePos)
                            {
                                if (koro.cellPos == move)
                                {
                                    isMat = false;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }

        return isMat;
    }

    public bool CheckMat(Selectable pawn)
    {
        bool isMat = false;
        if(pawn is Koropokkuru)
		{
            foreach (Selectable p in allPieces)
			{
                if(!p.transform.CompareTag(pawn.tag) && !p.isDead)
				{
                    foreach(Vector2Int mov in p.movablePos)
					{
                        if (mov == pawn.cellPos)
                            isMat = true;
					}
				}
			}
		}

        return isMat;
    }

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

    public List<IPawn> GetReservePawnsByPlayer(ECampType campType)
    {
        throw new System.NotImplementedException();
    }

    public List<IPawn> GetPawnsOnBoard(ECampType campType)
    {
        throw new System.NotImplementedException();
    }

    public SAction GetLastAction()
    {
        throw new System.NotImplementedException();
    }
}

[System.Serializable]
public class GamePiece
{
    public GameObject piecePrefab;
    public Vector2Int pieceCoord;
}
