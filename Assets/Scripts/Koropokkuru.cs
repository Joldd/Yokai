using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koropokkuru : Selectable
{
    public override void ShowDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUp(), Color.red);
        Board.Instance.changeTileColor(cellDown(), Color.red);
        Board.Instance.changeTileColor(cellRight(), Color.red);
        Board.Instance.changeTileColor(cellLeft(), Color.red);
        Board.Instance.changeTileColor(cellUpLeft(), Color.red);
        Board.Instance.changeTileColor(cellUpRight(), Color.red);
        Board.Instance.changeTileColor(cellDownLeft(), Color.red);
        Board.Instance.changeTileColor(cellDownRight(), Color.red);
    }

    public override void HideDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUp(), Color.white);
        Board.Instance.changeTileColor(cellDown(), Color.white);
        Board.Instance.changeTileColor(cellRight(), Color.white);
        Board.Instance.changeTileColor(cellLeft(), Color.white);
        Board.Instance.changeTileColor(cellUpLeft(), Color.white);
        Board.Instance.changeTileColor(cellUpRight(), Color.white);
        Board.Instance.changeTileColor(cellDownLeft(), Color.white);
        Board.Instance.changeTileColor(cellDownRight(), Color.white);
    }
}
