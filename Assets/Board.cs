using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tileMap;

    public static Board Instance { get; private set; }

    public Selectable currentPiece;

    [SerializeField] Camera _camera;

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
    }

    public Vector3Int getTilePos(Vector3 pos)
    {
        return tileMap.WorldToCell(pos);
    }

    public void changeTileColor(Vector3Int pos, Color color)
    {
        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, color);
    }

    private void Update()
    {
        if (currentPiece != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < currentPiece.movablePos.Count; i++)
                {
                    Vector3Int movable = currentPiece.movablePos[i];
                    if (movable == getTilePos(_camera.ScreenToWorldPoint(Input.mousePosition)))
                    {
                        currentPiece.MoveTo(movable);
                        currentPiece.HideDeplacements();
                        currentPiece = null;
                        break;
                    }
                }
            }          
        }
    }
}
