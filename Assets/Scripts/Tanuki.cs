using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanuki : Selectable
{
    public override void ShowDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUp(), Color.red);
        Board.Instance.changeTileColor(cellDown(), Color.red);
        Board.Instance.changeTileColor(cellRight(), Color.red);
        Board.Instance.changeTileColor(cellLeft(), Color.red);
    }

    public override void HideDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUp(), Color.white);
        Board.Instance.changeTileColor(cellDown(), Color.white);
        Board.Instance.changeTileColor(cellRight(), Color.white);
        Board.Instance.changeTileColor(cellLeft(), Color.white);
    }
}
