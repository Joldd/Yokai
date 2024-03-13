using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
				TempPawn tempPawn = new TempPawn(pawn.cardType, true, pawn.cellPos);
				if (tempPawn.pawnType == PawnType.KODAMA && (pawn as Kodama).isSamourai) tempPawn.pawnType = PawnType.SAMURAI;

				tempBoard.Add(tempPawn);
			}
			else if (pawn.transform.CompareTag("Player02") && !pawn.isDead)
			{
				TempPawn tempPawn = new TempPawn(pawn.cardType, false, pawn.cellPos);
				if (tempPawn.pawnType == PawnType.KODAMA && (pawn as Kodama).isSamourai) tempPawn.pawnType = PawnType.SAMURAI;

				tempBoard.Add(tempPawn);
			}
		}

		foreach(TempPawn pawn in tempBoard)
		{
			pawn.SetMovablePos(tempBoard);
		}

		//DrawBoard(tempBoard);

		int i = Minimax(6, true, tempBoard, System.Int32.MinValue, System.Int32.MaxValue);
		Debug.Log(i);
	}

	private int Minimax(int depth, bool maximizingPlayer, List<TempPawn> temp, int alpha, int beta)
	{
		List<TempPawn> myTempBoard = CopyList(temp);
		DrawBoard(myTempBoard);
		if (depth == 0 /* OR if game is over*/)
		{
			int total = 0;
			Debug.Log("Count Board :" + myTempBoard.Count);
			foreach (TempPawn pawn in myTempBoard)
			{
				total += (int)pawn.pawnType * (pawn.isEnemy ? -1 : 1);
			}
			Debug.Log("Total: " + total);
			return total;
		}

		if (maximizingPlayer)
		{
			int maxEval = System.Int32.MinValue;
			foreach(TempPawn pawn in myTempBoard)
			{
				myTempBoard = CopyList(temp);
				if (pawn.isEnemy)
				{
					foreach(Vector2Int move in pawn.movablePos)
					{
						List<TempPawn> temp2 = CopyList(myTempBoard);
						temp2 = Move(pawn.currentPos, move, temp2);

						foreach (TempPawn p in temp2)
						{
							p.SetMovablePos(temp2);
						}

						int eval = Minimax(depth - 1, false, temp2, alpha, beta);
						maxEval = Mathf.Max(maxEval, eval);
						alpha = Mathf.Max(alpha, eval);
						if (beta <= alpha) break;
					}
				}
				if (beta <= alpha) break;
			}
			return maxEval;
		}
		else
		{
			int minEval = System.Int32.MaxValue;
			foreach (TempPawn pawn in myTempBoard)
			{
				myTempBoard = CopyList(temp);
				if (!pawn.isEnemy)
				{
					foreach (Vector2Int move in pawn.movablePos)
					{
						List<TempPawn> temp2 = CopyList(myTempBoard);
						temp2 = Move(pawn.currentPos, move, temp2);

						foreach (TempPawn p in temp2)
						{
							p.SetMovablePos(temp2);
						}

						int eval = Minimax(depth - 1, true, temp2, alpha, beta);
						minEval = Mathf.Min(minEval, eval);
						beta = Mathf.Min(beta, eval);
						if (beta <= alpha) break;
					}
				}
				if (beta <= alpha) break;
			}
			return minEval;
		}
	}

	private List<TempPawn> Move(Vector2Int pawnPos, Vector2Int pawnMove, List<TempPawn> myTempBoard)
	{
		List<TempPawn> temp = CopyList(myTempBoard);
		List<TempPawn> temp2 = CopyList(myTempBoard);
		for (int i = 0; i < temp2.Count; i++)
		{
			if(temp[i].currentPos == pawnMove)
			{
				temp[i] = null;
			}
		}
		temp.RemoveAll(x => x == null);

		temp2 = temp;

		foreach(TempPawn pawn in temp2)
		{
			if(pawn.currentPos == pawnPos)
			{
				pawn.currentPos = pawnMove;
			}
		}

		return temp2;
	}

	private void DrawBoard(List<TempPawn> board)
	{
		string boardDisplay = "\n";
		for (int i = 3; i >= 0; i--)
		{
			for (int j = 0; j <= 2; j++)
			{
				string s = "0 |";
				foreach (TempPawn temp in board)
				{
					if (temp.currentPos == new Vector2Int(j, i))
					{
						if (temp.isEnemy) s = ((int)temp.pawnType).ToString() + ":X |";
						else s = ((int)temp.pawnType).ToString() + ":O |";

					}
				}
				boardDisplay += s;
			}
			boardDisplay += "\n";
		}
		Debug.Log(boardDisplay);
	}

	List<TempPawn> CopyList(List<TempPawn> originalList)
	{
		List<TempPawn> copiedList = new List<TempPawn>();

		foreach (TempPawn originalObject in originalList)
		{
			// Create a new instance of CustomObject and copy the values
			TempPawn copiedObject = new TempPawn(originalObject.pawnType, originalObject.isEnemy, originalObject.currentPos, originalObject.movablePos);

			// Add the copied object to the new list
			copiedList.Add(copiedObject);
		}

		return copiedList;
	}
}

[System.Serializable]
public class TempPawn
{
	public PawnType pawnType;
	public bool isEnemy;
	public Vector2Int currentPos;
	public List<Vector2Int> movablePos = new List<Vector2Int>();

	public TempPawn(PawnType pawnType, bool isEnemy, Vector2Int currentPos)
	{
		this.pawnType = pawnType;
		this.isEnemy = isEnemy;
		this.currentPos = currentPos;
	}

	public TempPawn(PawnType pawnType, bool isEnemy, Vector2Int currentPos, List<Vector2Int> movablePos)
	{
		this.pawnType = pawnType;
		this.isEnemy = isEnemy;
		this.currentPos = currentPos;
		this.movablePos = new List<Vector2Int>(movablePos);
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
}
