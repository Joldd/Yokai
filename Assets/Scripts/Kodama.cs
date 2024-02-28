using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kodama : Selectable
{
    public override void OnMouseDown()
    {
        Vector3Int h = Board.Instance.getTilePos(transform.position);
        h.y += 1;
        Board.Instance.changeTileColor(h);
    }
}
