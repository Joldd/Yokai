using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Tilemap tileMap;

    [SerializeField] private GamePiece[] gamePieces;

    public List<Selectable> allPieces = new List<Selectable>();

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
                if (i == 0)
                {
                    go.transform.tag = "Player01";
                    go.transform.position = tileMap.GetCellCenterWorld(gp.pieceCoord);
                    //Board.Instance.currentBoard[gp.pieceCoord.y].currentRowBoard[gp.pieceCoord.x] = 1;
                    Board.Instance.tileMap.SetTileFlags(gp.pieceCoord, TileFlags.None);
                    Board.Instance.getCustomTile(gp.pieceCoord).cardOnTile = go.GetComponent<Selectable>();
                }
                else
                {
                    go.transform.tag = "Player02";
                    go.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(2, 3, 0) - gp.pieceCoord);
                    //Board.Instance.currentBoard[3 - gp.pieceCoord.y].currentRowBoard[2 - gp.pieceCoord.x] = 2;
                    Board.Instance.tileMap.SetTileFlags(gp.pieceCoord, TileFlags.None);
                    Board.Instance.getCustomTile(new Vector3Int(2, 3, 0) - gp.pieceCoord).cardOnTile = go.GetComponent<Selectable>();
                    go.transform.eulerAngles = new Vector3(0, 0, 180);
                }
                allPieces.Add(go.GetComponent<Selectable>());
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
}

[System.Serializable]
public class GamePiece
{
    public GameObject piecePrefab;
    public Vector3Int pieceCoord;
}
