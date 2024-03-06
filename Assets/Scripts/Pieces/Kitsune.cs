using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Kitsune : Selectable
{
    public override void ShowDeplacements()
    {
        //Current position on board
        base.ShowDeplacements();

        //Red tiles
        foreach (Vector2Int mov in movablePos)
        {
            Board.Instance.changeTileColor(mov, Color.red);
        }
    }

    public override void addAllMovable()
    {
        base.addAllMovable();

        //Add possibles movements
        addMovablePos(cellUpLeft());
        addMovablePos(cellUpRight());
        addMovablePos(cellDownLeft());
        addMovablePos(cellDownRight());
    }
}
