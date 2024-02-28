using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public Vector3Int cellPos;
    public List<Vector3Int> movablePos = new List<Vector3Int>();

    public virtual void OnMouseDown()
    {
        if (Board.Instance.currentPiece != null)
        {
            Board.Instance.currentPiece.HideDeplacements();
        }

        ShowDeplacements();

        if (Board.Instance.currentPiece == this)
        {
            HideDeplacements();
            Board.Instance.currentPiece = null;
        }
        else
        {
            Board.Instance.currentPiece = this;
        }
    }

    public virtual void MoveTo(Vector3Int pos)
    {
        transform.position = Board.Instance.tileMap.GetCellCenterWorld(pos);
    }

    public virtual void ShowDeplacements()
    {
        cellPos = Board.Instance.getTilePos(transform.position);
    }

    public virtual void HideDeplacements()
    {
        
    }

    public virtual Vector3Int cellUp()
    {
        return new Vector3Int(cellPos.x, cellPos.y + 1, 0);
    }

    public virtual Vector3Int cellDown()
    {
        return new Vector3Int(cellPos.x, cellPos.y - 1, 0);
    }

    public virtual Vector3Int cellLeft()
    {
        return new Vector3Int(cellPos.x - 1, cellPos.y, 0);
    }

    public virtual Vector3Int cellRight()
    {
        return new Vector3Int(cellPos.x + 1, cellPos.y, 0);
    }

    public virtual Vector3Int cellUpLeft()
    {
        return new Vector3Int(cellPos.x - 1, cellPos.y + 1, 0);
    }

    public virtual Vector3Int cellUpRight()
    {
        return new Vector3Int(cellPos.x + 1, cellPos.y + 1, 0);
    }

    public virtual Vector3Int cellDownLeft()
    {
        return new Vector3Int(cellPos.x - 1, cellPos.y - 1, 0);
    }

    public virtual Vector3Int cellDownRight()
    {
        return new Vector3Int(cellPos.x + 1, cellPos.y - 1, 0);
    }
}
