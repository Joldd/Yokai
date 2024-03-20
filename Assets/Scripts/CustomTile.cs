using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using YokaiNoMori.Interface;

[CreateAssetMenu(fileName = "CustomTile", menuName = "Tiles/Custom")]
public class CustomTile : Tile, IBoardCase
{
	[Header("CustomProperty")]
	public UnityEvent clickAction;

	public Selectable cardOnTile;

	public Vector2Int tilePosition;

	public void OnClickTile()
	{
		clickAction.Invoke();
		clickAction.RemoveAllListeners();
	}

	//Interface IBoardCase
	public IPawn GetPawnOnIt()
	{
		return cardOnTile;
	}

	public Vector2Int GetPosition()
	{
		return tilePosition;
	}

    public bool IsBusy()
    {
		if (cardOnTile != null)
		{
			return true;
		}
		else return false;
    }
}
