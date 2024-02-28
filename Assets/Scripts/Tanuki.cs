using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanuki : Selectable
{
    public override void OnMouseDown()
    {
        Vector3Int h = Board.Instance.getTilePos(transform.position);
        h.y += 1;
        Board.Instance.changeTileColor(h);
        h.y -= 2;
        Board.Instance.changeTileColor(h);
        h.y += 1;
        h.x -= 1;
        Board.Instance.changeTileColor(h);
        h.x += 2;
        Board.Instance.changeTileColor(h);
    }
}
