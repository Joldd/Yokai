using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kodama : Selectable
{
    public override void ShowDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUp(), Color.red);
    }

    public override void HideDeplacements()
    {
        base.ShowDeplacements();
        Board.Instance.changeTileColor(cellUp(), Color.white);
    }
}
