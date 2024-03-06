using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kodama : Selectable
{
    private SpriteRenderer _spriteRenderer;
    public Sprite _samourai;
    public Sprite _normal;

    public bool isSamourai;

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
            addMovablePos(cellUp());
            if (isSamourai)
            {
                addMovablePos(cellUpLeft());
                addMovablePos(cellUpRight());
                addMovablePos(cellLeft());
                addMovablePos(cellRight());
                addMovablePos(cellDown());
            }
        }
        else
        {
            addMovablePos(cellDown());
            if (isSamourai)
            {
                addMovablePos(cellDownLeft());
                addMovablePos(cellDownRight());
                addMovablePos(cellLeft());
                addMovablePos(cellRight());
                addMovablePos(cellUp());
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
