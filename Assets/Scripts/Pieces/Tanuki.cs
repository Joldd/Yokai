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
