using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField] Tilemap tileMap;
    Vector3Int location;
    [SerializeField] Tile tileRed;

    public static Board Instance { get; private set; }

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

    public void changeTileColor(Vector3Int pos)
    {
        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, Color.red);
    }
}
