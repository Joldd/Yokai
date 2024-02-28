using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitsune : Selectable
{
    public override void ShowDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUpLeft(), Color.red);
        Board.Instance.changeTileColor(cellUpRight(), Color.red);
        Board.Instance.changeTileColor(cellDownLeft(), Color.red);
        Board.Instance.changeTileColor(cellDownRight(), Color.red);
    }

    public override void HideDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUpLeft(), Color.white);
        Board.Instance.changeTileColor(cellUpRight(), Color.white);
        Board.Instance.changeTileColor(cellDownLeft(), Color.white);
        Board.Instance.changeTileColor(cellDownRight(), Color.white);
    }
}
