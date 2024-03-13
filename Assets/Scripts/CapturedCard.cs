using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;


public class CapturedCard : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private int player;
	private Selectable currentCard;

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

	public void SetCapturedPiece(Selectable card, int player)
	{
		card.capturedCard = this;
		this.currentCard = card;
		this.player = player;

		if(currentCard is Kodama)
		{
			Kodama kodama = currentCard as Kodama;
			image.sprite = kodama._normal;
			kodama.isSamourai = false;
			kodama._spriteRenderer.sprite = kodama._normal;
		}
		else
			image.sprite = card.GetComponent<SpriteRenderer>().sprite;

		if (this.player == 1)
		{
			image.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
			currentCard.transform.SetParent(GameManager.Instance.player01);
			currentCard.transform.tag = currentCard.transform.parent.tag;
			currentCard.transform.eulerAngles = new Vector3(0, 0, 0);
		}
		else
		{
			image.gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
			currentCard.transform.SetParent(GameManager.Instance.player02);
			currentCard.transform.tag = currentCard.transform.parent.tag;
			currentCard.transform.eulerAngles = new Vector3(0, 0, 180);
		}
	}

	public void OnClickPiece()
	{
		//TO DO display available tile
		Board.Instance.currentPiece = null;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Vector2Int pos = new Vector2Int(j, i);

				Board.Instance.getCustomTile(pos).clickAction.RemoveAllListeners();
				Board.Instance.changeTileColor(pos, Color.white);

				if (Board.Instance.getCustomTile(pos) != null && Board.Instance.getCustomTile(new Vector2Int(j, i)).cardOnTile == null)
				{
					Board.Instance.changeTileColor(pos, Color.red);
					Board.Instance.getCustomTile(pos).clickAction.RemoveAllListeners();
					Board.Instance.getCustomTile(pos).clickAction.AddListener(() => {
						InvokePiece(pos);
					});

				}
			}
		}
	}

	public void InvokePiece(Vector2Int pos)
	{
		//currentCard.gameObject.SetActive(true);
		currentCard.transform.position = Board.Instance.tileMap.GetCellCenterWorld((Vector3Int)pos);
		Selectable selectable = currentCard.GetComponent<Selectable>();
		selectable.isDead = false;
		Board.Instance.getCustomTile(pos).cardOnTile = selectable;
		selectable.cellPos = pos;
        GameManager.Instance.UpdateAllMovablePos();
        Destroy(this.gameObject);
        Board.Instance.ClearTile();
        Board.Instance.changeTurn.Invoke();
    }
}
