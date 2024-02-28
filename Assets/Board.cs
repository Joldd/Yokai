using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    private Tilemap tileMap;

    public static Board Instance { get; private set; }

    public Selectable currentPiece;

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
}
