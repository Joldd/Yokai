using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SocialPlatforms.Impl;
using System.IO;

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

	public TempPawn bestPawn;
    public Vector2Int bestMove;
	public bool IAmove;
	public bool IAparachute;
	public int maxDepth = 4;

    [SerializeField] List<TempPawn> tempBoard = new List<TempPawn>();

    [SerializeField] List<TempPawn> diedBoard = new List<TempPawn>();

    [ContextMenu("THINK")]
	public void Think()
	{
		//LIVING BOARD
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

        foreach (TempPawn pawn in tempBoard)
		{
			pawn.SetMovablePos(tempBoard);
		}

        //DIED BOARD
		diedBoard.Clear();
        foreach (Selectable pawn in GameManager.Instance.allPieces)
        {
            if (pawn.transform.CompareTag("Player01") && pawn.isDead)
            {
                TempPawn tempPawn = new TempPawn(pawn.cardType, true, pawn.cellPos);
                if (tempPawn.pawnType == PawnType.KODAMA && (pawn as Kodama).isSamourai) tempPawn.pawnType = PawnType.SAMURAI;
                diedBoard.Add(tempPawn);
            }
            else if (pawn.transform.CompareTag("Player02") && pawn.isDead)
            {
                TempPawn tempPawn = new TempPawn(pawn.cardType, false, pawn.cellPos);
                if (tempPawn.pawnType == PawnType.KODAMA && (pawn as Kodama).isSamourai) tempPawn.pawnType = PawnType.SAMURAI;
                diedBoard.Add(tempPawn);
            }
        }

        int depth = maxDepth;

		int i = Minimax(depth, true, tempBoard, diedBoard, System.Int32.MinValue, System.Int32.MaxValue);
    }

	private int Minimax(int depth, bool maximizingPlayer, List<TempPawn> temp, List<TempPawn> tempDie, int alpha, int beta)
	{
		List<TempPawn> myTempBoard = CopyList(temp);
		ScoreBool mat = CheckMatPawn(myTempBoard, maximizingPlayer);
		ScoreBool victory = CheckVictory(myTempBoard, maximizingPlayer);
        List<TempPawn> myDieBoard = CopyList(tempDie);
        List<Vector2Int> myParachutePos = new List<Vector2Int>();

		int kN = 0;
		foreach (TempPawn pawn in myTempBoard)
		{
			if (pawn.pawnType == PawnType.KOROPOKKURU) kN++;
		}

		if (depth <= 0 || ((mat.isEnded || victory.isEnded) && depth != maxDepth) || kN < 2)
		{
			int total = 0;
			foreach (TempPawn pawn in myTempBoard)
			{
				total += (int)pawn.pawnType * (pawn.isEnemy ? -1 : 1);
			}
			if (mat.isEnded) total += mat.scoreAmount;
			if (victory.isEnded)total += victory.scoreAmount;

			//if (depth <= 0)
			//{
			//	Debug.Log($"Ended at lower depth");
			//	DrawBoard(myTempBoard);
			//	Debug.Log("____");
			//}
			//else if (mat.isEnded)
			//{
			//	Debug.Log($"Ended with mat at: {depth}, with score: {total}");
			//	DrawBoard(myTempBoard);
			//	Debug.Log("____");
			//}
			//else if (victory.isEnded)
			//{
			//	Debug.Log($"Ended with victory at: {depth}, with score: {total}");
			//	DrawBoard(myTempBoard);
			//	Debug.Log("____");
			//}

			return total;
		}

		if (maximizingPlayer)
		{
			int maxEval = System.Int32.MinValue;
			foreach (TempPawn pawn in myTempBoard)
			{
				myTempBoard = CopyList(temp);
				myDieBoard = CopyList(tempDie);
				if (!pawn.isEnemy)
				{
					foreach (Vector2Int move in pawn.movablePos)
					{
						List<TempPawn> temp2 = CopyList(myTempBoard);
						List<TempPawn> tempDie2 = CopyList(myDieBoard);
						temp2 = Move(pawn.currentPos, move, temp2, tempDie2);

						foreach (TempPawn p in temp2)
						{
							p.SetMovablePos(temp2);
						}

						int eval = Minimax(depth - 1, false, temp2, tempDie2, alpha, beta);

          //              CSVManager.SaveData(
          //                          new string[7] {
										//depth.ToString(),
          //                              pawn.pawnType.ToString(),
          //                              move.ToString(),
										//"Move",
          //                              eval.ToString(),
										//"IA",
										//GameManager.Instance.turn.ToString()
          //                          });

                        if (eval > maxEval)
						{
							maxEval = eval;

							if (depth == maxDepth)
							{
								bestPawn = pawn;
								bestMove = move;

								IAmove = true;
								IAparachute = false;
							}
						}
						alpha = Mathf.Max(alpha, eval);
						if (beta <= alpha) break;
					}
				}
				if (beta <= alpha) break;
			}
			foreach (TempPawn pawn in myDieBoard)
			{
				if (!pawn.isEnemy)
				{
					myTempBoard = CopyList(temp);
					myDieBoard = CopyList(tempDie);
					myParachutePos = GetParachutePos(myTempBoard);
					foreach (Vector2Int newPos in myParachutePos)
					{
						List<TempPawn> temp2 = CopyList(myTempBoard);
						List<TempPawn> tempDie2 = CopyList(myDieBoard);
						Parachute(pawn, newPos, temp2, tempDie2);

						foreach (TempPawn p in temp2)
						{
							p.SetMovablePos(temp2);
						}

						int eval = Minimax(depth - 1, false, temp2, tempDie2, alpha, beta);

                        //CSVManager.SaveData(
                        //            new string[7] {
                        //                depth.ToString(),
                        //                pawn.pawnType.ToString(),
                        //                newPos.ToString(),
                        //                "Parachute",
                        //                eval.ToString(),
                        //                "IA",
                        //                GameManager.Instance.turn.ToString()
                        //            });

                        if (eval > maxEval)
						{
							maxEval = eval;

							if (depth == maxDepth)
							{
								bestPawn = pawn;
								bestMove = newPos;
								IAmove = false;
								IAparachute = true;
							}
						}
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
				myDieBoard = CopyList(tempDie);
				if (pawn.isEnemy)
				{
					foreach (Vector2Int move in pawn.movablePos)
					{
						List<TempPawn> temp2 = CopyList(myTempBoard);
						List<TempPawn> tempDie2 = CopyList(myDieBoard);
						temp2 = Move(pawn.currentPos, move, temp2, tempDie2);

						foreach (TempPawn p in temp2)
						{
							p.SetMovablePos(temp2);
						}

						int eval = Minimax(depth - 1, true, temp2, tempDie2, alpha, beta);

                        //CSVManager.SaveData(
                        //            new string[7] {
                        //                depth.ToString(),
                        //                pawn.pawnType.ToString(),
                        //                move.ToString(),
                        //                "Move",
                        //                eval.ToString(),
                        //                "Joueur",
                        //                GameManager.Instance.turn.ToString()
                        //            });

                        minEval = Mathf.Min(minEval, eval);
						beta = Mathf.Min(beta, eval);
						if (beta <= alpha) break;
					}
				}
				if (beta <= alpha) break;
			}
			foreach (TempPawn pawn in myDieBoard)
			{
				if (pawn.isEnemy)
				{
					myTempBoard = CopyList(temp);
					myDieBoard = CopyList(tempDie);
					myParachutePos = GetParachutePos(myTempBoard);
					foreach (Vector2Int newPos in myParachutePos)
					{
						List<TempPawn> temp2 = CopyList(myTempBoard);
						List<TempPawn> tempDie2 = CopyList(myDieBoard);
						Parachute(pawn, newPos, temp2, tempDie2);

						foreach (TempPawn p in temp2)
						{
							p.SetMovablePos(temp2);
						}

						int eval = Minimax(depth - 1, true, temp2, tempDie2, alpha, beta);

                        //CSVManager.SaveData(
                        //            new string[7] {
                        //                depth.ToString(),
                        //                pawn.pawnType.ToString(),
                        //                newPos.ToString(),
                        //                "Parachute",
                        //                eval.ToString(),
                        //                "Joueur",
                        //                GameManager.Instance.turn.ToString()
                        //            });

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

    private void Parachute(TempPawn pawn, Vector2Int posParachute, List<TempPawn> myTempBoard, List<TempPawn> dieBoard)
	{
		pawn.currentPos = posParachute;
		myTempBoard.Add(pawn);
		dieBoard.Remove(pawn);
	}

    private List<TempPawn> Move(Vector2Int pawnPos, Vector2Int pawnMove, List<TempPawn> myTempBoard, List<TempPawn> myDieBoard)
    {
        List<TempPawn> temp2 = CopyList(myTempBoard);

		for (int i = temp2.Count - 1; i >= 0; i--)
		{
			if (temp2[i].currentPos == pawnMove)
			{
				TempPawn tempPawn = temp2[i];
				temp2.RemoveAt(i);
				tempPawn.isEnemy = !tempPawn.isEnemy;
				myDieBoard.Add(tempPawn);
			}
		}

		foreach (TempPawn pawn in temp2)
        {
            if (pawn.currentPos == pawnPos)
            {
                pawn.currentPos = pawnMove;
				if (pawn.pawnType == PawnType.KODAMA && pawnPos.y == 3 && pawn.isEnemy)
				{
					pawn.pawnType = PawnType.SAMURAI;
				}
                if (pawn.pawnType == PawnType.KODAMA && pawnPos.y == 0 && !pawn.isEnemy)
                {
                    pawn.pawnType = PawnType.SAMURAI;
                }
            }
        }
        return temp2;
    }

    private void DrawBoard(List<TempPawn> board)
	{
		string test = "\n";
		foreach (TempPawn temp in board)
		{
			if (temp.isEnemy) test += ((int)temp.pawnType).ToString() + ":X |";
			else test += ((int)temp.pawnType).ToString() + ":O |";
		}

		string boardDisplay = test + "\n";
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

    private List<Vector2Int> GetParachutePos(List<TempPawn> board)
    {
		List<Vector2Int> L_ParachutePos = new List<Vector2Int>();
        for (int i = 3; i >= 0; i--)
        {
            for (int j = 0; j <= 2; j++)
            {
                Vector2Int pos = new Vector2Int(j, i);
                L_ParachutePos.Add(pos);
                foreach (TempPawn temp in board)
                {
                    if (temp.currentPos == new Vector2Int(j, i))
                    {
                        L_ParachutePos.Remove(pos);
                    }
                }
            }
        }
		return L_ParachutePos;
    }

    List<TempPawn> CopyList(List<TempPawn> originalList)
	{
		List<TempPawn> copiedList = new List<TempPawn>();

		foreach (TempPawn originalObject in originalList)
		{
			if (originalObject != null)
			{
                // Create a new instance of CustomObject and copy the values
                TempPawn copiedObject = new TempPawn(originalObject.pawnType, originalObject.isEnemy, originalObject.currentPos, originalObject.movablePos);

                // Add the copied object to the new list
                copiedList.Add(copiedObject);
            }
		}

		return copiedList;
	}

	ScoreBool CheckMatPawn(List<TempPawn> board, bool maximizing)
	{
		bool isMat = false;
		int score = 0;
		foreach (TempPawn pawn in board)
		{
			isMat = pawn.IsMat(board, maximizing);
			if (isMat)
			{
				score = 10000 * (pawn.isEnemy ? 1 : -1);
				break;
			}
		}
		return new ScoreBool(isMat, score);
	}

	ScoreBool CheckVictory(List<TempPawn> board, bool maximizing)
	{
		bool isVictory = false;
		int score = 0;
		foreach (TempPawn pawn in board)
		{
			if (pawn.pawnType == PawnType.KOROPOKKURU)
			{
				if ((pawn.isEnemy && pawn.currentPos.y == 3) || (!pawn.isEnemy && pawn.currentPos.y == 0))
				{
					isVictory = true;
					if (!pawn.IsMat(board, maximizing))
					{
						score = 10000 * (pawn.isEnemy ? -1 : 1);
					}
					else
					{
						score = 10000 * (pawn.isEnemy ? 1 : -1);
					}
					break;
				}
			}
		}
		return new ScoreBool(isVictory, score);
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

	public bool IsMat(List<TempPawn> tempBoard, bool maximizing)
	{
		if (pawnType != PawnType.KOROPOKKURU) return false;
		if (isEnemy != maximizing) return false;

        foreach (TempPawn pawn in tempBoard)
        {
            if (pawn.isEnemy != isEnemy)
			{
                foreach (Vector2Int move in pawn.movablePos)
                {
                    if (move == currentPos)
					{
						return true;
					}
                }
            }
        }
        return false;
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

public class ScoreBool
{
	public bool isEnded;
	public int scoreAmount;

	public ScoreBool(bool isEnded, int scoreAmount)
	{
		this.isEnded = isEnded;
		this.scoreAmount = scoreAmount;
	}
}
