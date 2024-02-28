using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapturedCard : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int player;

    public void SetCapturedPiece(GameObject prefab, int player)
	{
        this.prefab = prefab;
        this.player = player;
        image.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
	}

    public void OnClickPiece()
	{
		//TO DO display available tile
		for (int i = 0; i < Board.Instance.currentBoard.Length; i++)
		{
			for (int j = 0; j < Board.Instance.currentBoard[i].currentRowBoard.Length; j++)
			{
				if(Board.Instance.currentBoard[i].currentRowBoard[j] == 0)
				{
					Board.Instance.changeTileColor(new Vector3Int(j, i, 0), Color.red);
				}
			}
		}
	}
}
