using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kodama : Selectable
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _samourai;
    [SerializeField] private Sprite _normal;

    private bool isSamourai;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void ShowDeplacements()
    {
        //Current position on board
        base.ShowDeplacements();

        //Add possibles movements
        if (tag == "Player01")
        {
            movablePos.Add(cellUp());
            if (isSamourai)
            {
                movablePos.Add(cellUpLeft());
                movablePos.Add(cellUpRight());
                movablePos.Add(cellLeft());
                movablePos.Add(cellRight());
                movablePos.Add(cellDown());
            }
        }
        else
        {
            movablePos.Add(cellDown());
            if (isSamourai)
            {
                movablePos.Add(cellDownLeft());
                movablePos.Add(cellDownRight());
                movablePos.Add(cellLeft());
                movablePos.Add(cellRight());
                movablePos.Add(cellUp());
            }
        }

        //Red tiles
        foreach (Vector3Int mov in movablePos)
        {
            Board.Instance.changeTileColor(mov, Color.red);
        }
    }

    public override void HideDeplacements()
    {
        base.ShowDeplacements();

        //Clear color
        foreach (Vector3Int mov in movablePos)
        {
            Board.Instance.changeTileColor(mov, Color.white);
        }

        movablePos.Clear();
    }

    public override void MoveTo(Vector3Int pos)
    {
        base.MoveTo(pos);
        if ((tag == "Player01" && pos.y == 3) || (tag == "Player02" && pos.y == 0))
        {
            _spriteRenderer.sprite = _samourai;
            isSamourai = true;
        }
    }
}
