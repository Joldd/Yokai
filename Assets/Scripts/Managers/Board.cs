using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    public Tilemap tileMap;

    public static Board Instance { get; private set; }

    public Selectable currentPiece;

    [SerializeField] Camera _camera;
    //public BoardRow[] currentBoard;

    public bool isPlayer1Turn = true;
    [HideInInspector] public UnityEvent changeTurn;

    [SerializeField] UIManager uiManager;

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
        tileMap = GetComponent<Tilemap>();

        uiManager.UpdatePlayerTurn();

        changeTurn.AddListener(() =>
        {
            isPlayer1Turn = !isPlayer1Turn;
            uiManager.UpdatePlayerTurn();
        });
    }

    public Vector2Int getTilePos(Vector3 pos)
    {
        return (Vector2Int)tileMap.WorldToCell(pos);
    }

    public void changeTileColor(Vector2Int pos, Color color)
    {
        if(getCustomTile(pos) != null)
		{
            tileMap.SetTileFlags((Vector3Int)pos, TileFlags.None);
            tileMap.SetColor((Vector3Int)pos, color);
        }
    }

    public CustomTile getCustomTile(Vector2Int pos)
	{
        if (tileMap.GetTile((Vector3Int)pos) is CustomTile)
            return (CustomTile)tileMap.GetTile((Vector3Int)pos);
        return null;
    }

	private void OnMouseDown()
	{

	}

	private void Update()
    {
        if (GameManager.Instance.gamePaused) return;

		if (Input.GetMouseButtonDown(0))
		{
			if (tileMap.GetTile((Vector3Int)getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition))) is CustomTile)
			{
				CustomTile tile = (CustomTile)tileMap.GetTile((Vector3Int)getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)));

				tile.OnClickTile();
			}
		}

		if (currentPiece != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
				if (tileMap.GetTile((Vector3Int)getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition))) is CustomTile)
				{
					if (currentPiece.movablePos.Contains((Vector2Int)getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition))))
					{
                        GameManager.Instance.SaveBoards();
                        CustomTile tile = (CustomTile)tileMap.GetTile((Vector3Int)getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)));
                        tile.OnClickTile();
                        currentPiece.MoveTo(getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)));
                        currentPiece.HideDeplacements();
                        currentPiece = null;
                    }
                }
				//for (int i = 0; i < currentPiece.movablePos.Count; i++)
				//{
				//	Vector3Int movable = currentPiece.movablePos[i];
				//	if (movable == getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)))
				//	{
				//		currentPiece.MoveTo(movable);
				//		currentPiece.HideDeplacements();
				//		currentPiece = null;
				//		break;
				//	}
				//}
			}
        }
    }

    public void ClearTile()
	{
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pos = new Vector2Int(j, i);
                if (getCustomTile(pos) != null)
                {
                    changeTileColor(pos, Color.white);
                    getCustomTile(pos).clickAction.RemoveAllListeners();
                }
            }
        }
    }
}

[System.Serializable]
public class BoardRow
{
    public int[] currentRowBoard;
}
