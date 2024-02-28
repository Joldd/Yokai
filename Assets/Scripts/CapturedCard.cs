using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;


public class CapturedCard : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int player;

	private void Start()
	{
		if (Board.Instance.isPlayer1Turn)
			GetComponent<Button>().interactable = player == 1 ? true : false;
		else
			GetComponent<Button>().interactable = player == 1 ? false : true;

		Board.Instance.changeTurn.AddListener(() =>
		{
			if (Board.Instance.isPlayer1Turn)
				GetComponent<Button>().interactable = player == 1 ? true : false;
			else
				GetComponent<Button>().interactable = player == 1 ? false : true;
		});
	}

	public void SetCapturedPiece(int p, int player, Sprite sprite)
	{
		this.prefab = GameManager.Instance.gamePieces[p].piecePrefab;
		this.player = player;
        image.sprite = sprite;
	}

    public void OnClickPiece()
	{
		//TO DO display available tile
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Vector3Int pos = new Vector3Int(j, i, 0);
				if (Board.Instance.getCustomTile(pos) != null && Board.Instance.getCustomTile(new Vector3Int(j, i, 0)).cardOnTile == null)
				{
					Board.Instance.changeTileColor(pos, Color.red);
					Board.Instance.getCustomTile(pos).clickAction.RemoveAllListeners();
					Board.Instance.getCustomTile(pos).clickAction.AddListener(() => {
						InvokePiece(pos);
						Board.Instance.ClearTile();
						Board.Instance.isPlayer1Turn = !Board.Instance.isPlayer1Turn;
						Board.Instance.changeTurn.Invoke();
					});

				}
			}
		}
	}

	private void InvokePiece(Vector3Int pos)
	{
		GameObject go = Instantiate(prefab);
		go.transform.position = Board.Instance.tileMap.GetCellCenterWorld(pos);
		Board.Instance.getCustomTile(pos).cardOnTile = go.GetComponent<Selectable>();

		if (player == 1)
		{
			go.transform.tag = "Player01";
		}
		else
		{
			go.transform.tag = "Player02";
			go.transform.eulerAngles = new Vector3(0, 0, 180);
		}

		Destroy(this.gameObject);
	}
}
