using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PawnType
{
	KODAMA = 10,
	KITSUNE = 40,
	TANUKI = 50,
	SAMURAI = 90,
	KOROPOKKURU = 900
}

public class MinimaxAlgorithm : MonoBehaviour
{

	[SerializeField] List<TempPawn> tempBoard = new List<TempPawn>();

	[ContextMenu("THINK")]
	public void Think()
	{
		tempBoard.Clear();
		foreach (Selectable pawn in GameManager.Instance.allPieces)
		{
			if(pawn.transform.CompareTag("Player01") && !pawn.isDead)
			{
				TempPawn tempPawn = new TempPawn(pawn.cardType, true, pawn.cellPos, tempBoard);
				if (tempPawn.pawnType == PawnType.KODAMA && (pawn as Kodama).isSamourai) tempPawn.pawnType = PawnType.SAMURAI;

				tempBoard.Add(tempPawn);
			}
			else if (pawn.transform.CompareTag("Player02") && !pawn.isDead)
			{
				TempPawn tempPawn = new TempPawn(pawn.cardType, false, pawn.cellPos, tempBoard);
				if (tempPawn.pawnType == PawnType.KODAMA && (pawn as Kodama).isSamourai) tempPawn.pawnType = PawnType.SAMURAI;

				tempBoard.Add(tempPawn);
			}
		}

		foreach(TempPawn pawn in tempBoard)
		{
			pawn.SetMovablePos(tempBoard);
		}

		//int i = Minimax(5, true);
	}

	private int Minimax(int depth, bool maximizingPlayer)
	{
		if (depth == 0 /* OR if game is over*/) return 0;

		if (maximizingPlayer)
		{
			int maxEval = System.Int32.MinValue;
			foreach(TempPawn pawn in tempBoard)
			{
				if (pawn.isEnemy)
				{
					int eval = Minimax(depth - 1, false);
					maxEval = Mathf.Max(maxEval, eval);
				}
			}
			return maxEval;
		}
		else
		{
			int minEval = System.Int32.MaxValue;
			foreach (TempPawn pawn in tempBoard)
			{
				if (!pawn.isEnemy)
				{
					int eval = Minimax(depth - 1, false);
					minEval = Mathf.Min(minEval, eval);
				}
			}
			return minEval;
		}
	}
}

[System.Serializable]
public class TempPawn
{
	public PawnType pawnType;
	public bool isEnemy;
	public Vector2Int currentPos;
	public List<Vector2Int> movablePos = new List<Vector2Int>();

	public TempPawn(PawnType pawnType, bool isEnemy, Vector2Int currentPos, List<TempPawn> tempBoard)
	{
		this.pawnType = pawnType;
		this.isEnemy = isEnemy;
		this.currentPos = currentPos;
	}

	public void SetMovablePos(List<TempPawn> tempBoard)
	{
		int i = isEnemy ? 1 : -1;
		switch (pawnType)
		{
			case PawnType.KODAMA:
				movablePos.Add(new Vector2Int(currentPos.x, currentPos.y + 1 * i));
				break;
			case PawnType.KITSUNE:
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y + 1));
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y - 1));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y + 1));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y - 1));
				break;
			case PawnType.TANUKI:
				movablePos.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
				movablePos.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
				break;
			case PawnType.SAMURAI:
				movablePos.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
				movablePos.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y + 1 * i));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y + 1 * i));
				break;
			case PawnType.KOROPOKKURU:
				movablePos.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
				movablePos.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y + 1));
				movablePos.Add(new Vector2Int(currentPos.x + 1, currentPos.y - 1));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y + 1));
				movablePos.Add(new Vector2Int(currentPos.x - 1, currentPos.y - 1));
				break;
			default:
				break;
		}

		List<Vector2Int> tempMovablePos = new List<Vector2Int>(movablePos);

		foreach(Vector2Int pos in movablePos)
		{
			foreach (TempPawn pawn in tempBoard)
			{
				if (pawn.currentPos == pos)
				{
					if (pawn.isEnemy == isEnemy)
					{
						tempMovablePos.Remove(pos);
					}
				}
			}
			if(pos.x > 2 || pos.y > 3 || pos.x < 0 || pos.y < 0) tempMovablePos.Remove(pos);
		}

		movablePos.Clear();
		movablePos = new List<Vector2Int>(tempMovablePos);
	}

	public void Move()
	{
		//TO DO movement
		//TO DO killing
	}
}
