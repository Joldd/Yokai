using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Selectable : MonoBehaviour
{
    public Vector3Int cellPos;
    public List<Vector3Int> movablePos = new List<Vector3Int>();
    public bool isMoving;
    public Vector3 _target;
    public Vector3 _direction;
    public float _speed = 3f;

    public virtual void OnMouseDown()
    {
        if (tag == "Player01" && !Board.Instance.isPlayer1Turn) return;

        if (tag == "Player02" && Board.Instance.isPlayer1Turn) return;

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

    public void addMovablePos(Vector3Int pos)
    {
        if (Board.Instance.getCustomTile(pos) != null) movablePos.Add(pos);
    }

    public virtual void MoveTo(Vector3Int pos)
    {
        isMoving = true;
        _target = Board.Instance.tileMap.GetCellCenterWorld(pos);
        _direction = _target - transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (tag == "Player01")
            {
                transform.Translate(_speed * _direction * Time.deltaTime);
            }
            else
            {
                transform.Translate(-_speed * _direction * Time.deltaTime);
            }
            if (Vector3.Distance(transform.position, _target) <= 0.1f)
            {
                transform.position = _target;
                isMoving = false;
            }
        }
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
