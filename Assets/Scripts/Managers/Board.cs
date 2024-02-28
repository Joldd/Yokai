using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tileMap;

    public static Board Instance { get; private set; }

    public Selectable currentPiece;

    [SerializeField] Camera _camera;
    public BoardRow[] currentBoard;

    public bool isPlayer1Turn = true;

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
    }

    public Vector3Int getTilePos(Vector3 pos)
    {
        return tileMap.WorldToCell(pos);
    }

    public void changeTileColor(Vector3Int pos, Color color)
    {
        if(getCustomTile(pos) != null)
		{
            tileMap.SetTileFlags(pos, TileFlags.None);
            tileMap.SetColor(pos, color);
        }
    }

    public CustomTile getCustomTile(Vector3Int pos)
	{
        if (tileMap.GetTile(pos) is CustomTile)
            return (CustomTile)tileMap.GetTile(pos);

        return null;
    }

	private void OnMouseDown()
	{

	}

	private void Update()
    {
        //TEST
   //     if (Input.GetMouseButtonDown(0))
   //     {
   //         if (tileMap.GetTile(getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition))) is CustomTile)
   //         {
   //             CustomTile tile = (CustomTile)tileMap.GetTile(getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)));

   //             Debug.Log("pas trop nule");

   //             tile.clickAction.RemoveAllListeners();
   //             tile.clickAction.AddListener(() => { Debug.Log("Hello tile"); });
   //             tile.OnClickTile();
   //         }
			//else
			//{
   //             Debug.Log("nule");
   //         }
   //     }

        if (currentPiece != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("click 2 " + currentPiece.movablePos.Count);
				if (tileMap.GetTile(getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition))) is CustomTile)
				{
					if (currentPiece.movablePos.Contains(getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition))))
					{
                        CustomTile tile = (CustomTile)tileMap.GetTile(getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)));
                        tile.OnClickTile();
                        currentPiece.MoveTo(getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)));
                        currentPiece.HideDeplacements();
                        currentPiece = null;
                        isPlayer1Turn = !isPlayer1Turn;
                        uiManager.UpdatePlayerTurn();
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
}

[System.Serializable]
public class BoardRow
{
    public int[] currentRowBoard;
}
