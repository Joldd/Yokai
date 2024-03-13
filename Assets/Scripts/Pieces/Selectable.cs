using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;
using YokaiNoMori.Interface;
using YokaiNoMori.Enumeration;

public class Selectable : MonoBehaviour, IPawn
{
    public Vector2Int cellPos;
    public List<Vector2Int> movablePos = new List<Vector2Int>();
    public bool isMoving;
    public Vector3 _target;
    public Vector3 _direction;
    public float _speed = 3f;
    public bool isDead = false;

    [SerializeField] private GameObject capturedPrefab;
    public int cardType;

	public virtual void OnMouseDown()
    {
        if (GameManager.Instance.gamePaused) return;

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

    public void addMovablePos(Vector2Int pos)
    {
        CustomTile customTile = Board.Instance.getCustomTile(new Vector2Int(pos.x, pos.y));
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

    public virtual void addAllMovable()
    {
        movablePos.Clear();
    }

    public virtual void MoveTo(Vector2Int pos)
    {
        Board.Instance.getCustomTile(cellPos).cardOnTile = null;
        isMoving = true;
        _target = Board.Instance.tileMap.GetCellCenterWorld((Vector3Int)pos);
        _direction = _target - transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance.gamePaused) return;

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
                        go.GetComponent<CapturedCard>().SetCapturedPiece(cardOnTile, player + 1);
                        GameManager.Instance.killUnit(cardOnTile);
                    }
                }
                Board.Instance.getCustomTile(cellPos).cardOnTile = this;

                //Update movable pos of all pieces
                GameManager.Instance.UpdateAllMovablePos();

                //Check repetition to draw
                GameManager.Instance.CheckRepetition();

                //Check if Koropokkuru cross the line and won
                if (GameManager.Instance.CheckMat(GameManager.Instance.player01, GameManager.Instance.player02))
                {
                    UIManager.Instance.Victory(1);
                }
                else if (GameManager.Instance.CheckMat(GameManager.Instance.player02, GameManager.Instance.player01))
                {
                    UIManager.Instance.Victory(2);
                }

                //Check if Koropokkuru is Mat after a movement
                if (GameManager.Instance.CheckMat(this))
                    UIManager.Instance.Victory(this.tag == "Player01" ? 2 : 1);
            }
        }
    }

    public virtual void ShowDeplacements()
    {
        cellPos = (Vector2Int)Board.Instance.getTilePos(transform.position);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pos = new Vector2Int(j, i);

                Board.Instance.getCustomTile(pos).clickAction.RemoveAllListeners();
                Board.Instance.changeTileColor(pos, Color.white);
            }
        }
    }

    public virtual void HideDeplacements()
    {
        //Clear color
        foreach (Vector2Int mov in movablePos)
        {
            Board.Instance.changeTileColor(mov, Color.white);
        }
    }

    public virtual Vector2Int cellUp()
    {
        return new Vector2Int(cellPos.x, cellPos.y + 1);
    }

    public virtual Vector2Int cellDown()
    {
        return new Vector2Int(cellPos.x, cellPos.y - 1);
    }

    public virtual Vector2Int cellLeft()
    {
        return new Vector2Int(cellPos.x - 1, cellPos.y);
    }

    public virtual Vector2Int cellRight()
    {
        return new Vector2Int(cellPos.x + 1, cellPos.y);
    }

    public virtual Vector2Int cellUpLeft()
    {
        return new Vector2Int(cellPos.x - 1, cellPos.y + 1);
    }

    public virtual Vector2Int cellUpRight()
    {
        return new Vector2Int(cellPos.x + 1, cellPos.y + 1);
    }

    public virtual Vector2Int cellDownLeft()
    {
        return new Vector2Int(cellPos.x - 1, cellPos.y - 1);
    }

    public virtual Vector2Int cellDownRight()
    {
        return new Vector2Int(cellPos.x + 1, cellPos.y - 1);
    }

    //Interface IPawn
	public List<Vector2Int> GetDirections()
	{
        return movablePos;
	}

	public ICompetitor GetCurrentOwner()
	{
		throw new System.NotImplementedException();
	}

	public IBoardCase GetCurrentBoardCase()
	{
        return Board.Instance.getCustomTile(cellPos);

    }

	public EPawnType GetPawnType()
	{
        if(this is Kodama)
		{
            if((this as Kodama).isSamourai)
                return EPawnType.KodamaSamurai;
			else
                return EPawnType.Kodama;
        }
        else if(this is Kitsune)
            return EPawnType.Kitsune;
        else if(this is Tanuki)
            return EPawnType.Tanuki;
        else if(this is Koropokkuru)
            return EPawnType.Koropokkuru;

		throw new System.NotImplementedException();
	}
}
