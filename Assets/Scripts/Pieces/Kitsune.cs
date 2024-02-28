using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitsune : Selectable
{
    public override void ShowDeplacements()
    {
        //Current position on board
        base.ShowDeplacements();

        //Add possibles movements
        movablePos.Add(cellUpLeft());
        movablePos.Add(cellUpRight());
        movablePos.Add(cellDownLeft());
        movablePos.Add(cellDownRight());

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
}