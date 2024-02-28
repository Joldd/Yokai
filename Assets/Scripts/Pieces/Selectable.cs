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

    [SerializeField] private GameObject capturedPrefab;
    [SerializeField] private int cardType;

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
        CustomTile customTile = Board.Instance.getCustomTile(pos);
        if (customTile != null)
        {
            if (customTile.cardOnTile == null)
            {
                movablePos.Add(pos);
            }
            else if (customTile.cardOnTile.tag != tag)
            {
                movablePos.Add(pos);
            }
        }
    }

    public virtual void MoveTo(Vector3Int pos)
    {
        Board.Instance.getCustomTile(cellPos).cardOnTile = null;
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
                cellPos = Board.Instance.getTilePos(transform.position);
                Selectable cardOnTile = Board.Instance.getCustomTile(cellPos).cardOnTile;
                if (cardOnTile != null)
                {
                    if (cardOnTile.tag != tag)
                    {
                        if (cardOnTile.TryGetComponent<Koropokkuru>(out Koropokkuru card)){
                            if (tag == "Player01")
                            {
                                UIManager.Instance.Victory(1);
                            }
                            else
                            {
                                UIManager.Instance.Victory(2);
                            }
                        }
                        int player = tag == "Player01" ? 0 : 1;
                        Transform parent = UIManager.Instance.capturedPanel[player];
                        GameObject go = Instantiate(capturedPrefab, parent);
                        go.GetComponent<CapturedCard>().SetCapturedPiece(cardOnTile.cardType, player + 1, cardOnTile.GetComponent<SpriteRenderer>().sprite);
                        Destroy(cardOnTile.gameObject);
                        GameManager.Instance.killUnit(cardOnTile);
                    }
                }
                Board.Instance.getCustomTile(cellPos).cardOnTile = this;
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
