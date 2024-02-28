using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CustomTile", menuName = "Tiles/Custom")]
public class CustomTile : Tile
{
	[Header("CustomProperty")]
	public UnityEvent clickAction;

	public Selectable cardOnTile;

	public void OnClickTile()
	{
		clickAction.Invoke();
		clickAction.RemoveAllListeners();
	}
}
