using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public enum CardOnTile
{
	NEUTRAL = 0,
	PLAYER01 = 1,
	PLAYER02 = 2
}

[CreateAssetMenu(fileName = "CustomTile", menuName = "Tiles/Custom")]
public class CustomTile : Tile
{
	[Header("CustomProperty")]
	public UnityEvent clickAction;
	public CardOnTile cardOnTile;

	public void OnClickTile()
	{
		clickAction.Invoke();
		clickAction.RemoveAllListeners();
	}
}
