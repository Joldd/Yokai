using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanuki : Selectable
{
    public override void ShowDeplacements()
    {
        //Current position on board
        base.ShowDeplacements();

        //Add possibles movements
        addMovablePos(cellUp());
        addMovablePos(cellDown());
        addMovablePos(cellRight());
        addMovablePos(cellLeft());

        //Red tiles
        foreach (Vector2Int mov in movablePos)
        {
            Board.Instance.changeTileColor(mov, Color.red);
        }
    }
}
