using System.Collections.Generic;
using UnityEngine;

namespace Groupe10
{
    public class IAManager : MonoBehaviour
    {
        public static IAManager Instance { get; private set; }

        private List<Selectable> L_Pawns = new List<Selectable>();
        private List<CapturedCard> L_CapturedPawns = new List<CapturedCard>();

        public bool canPlay;

        private MinimaxAlgorithm minimaxAlgorithm;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            minimaxAlgorithm = GetComponent<MinimaxAlgorithm>();
        }

        private void Update()
        {
            ////////////////////// RANDOM IA  /////////////////////////////////////////////
            //if (!Board.Instance.isPlayer1Turn && GameManager.Instance.isIA && canPlay)
            //{
            //    if (ChooseMove()){
            //        Selectable myPawn = PickMovablePawn();
            //        int r = Random.Range(0, myPawn.movablePos.Count);
            //        myPawn.MoveTo(myPawn.movablePos[r]);
            //    }
            //    else
            //    {
            //        int randomPawn = Random.Range(0,L_CapturedPawns.Count);
            //        int randomPos = Random.Range(0, Board.Instance.GetFreePos().Count);
            //        L_CapturedPawns[randomPawn].InvokePiece(Board.Instance.GetFreePos()[randomPos]);
            //    }
            //    canPlay = false;
            //}

            ////////////////////// MINIMAX IA /////////////////////////////////////////////
            if (!Board.Instance.isPlayer1Turn && GameManager.Instance.isIA && canPlay && !GameManager.Instance.isGameOver)
            {
                minimaxAlgorithm.Think();
                if (minimaxAlgorithm.IAmove && !minimaxAlgorithm.IAparachute)
                {
                    GameManager.Instance.GetPawn(minimaxAlgorithm.bestPawn.currentPos).MoveTo(minimaxAlgorithm.bestMove);
                }
                else if (!minimaxAlgorithm.IAmove && minimaxAlgorithm.IAparachute)
                {
                    GameManager.Instance.GetCapturedCard(minimaxAlgorithm.bestPawn).InvokePiece(minimaxAlgorithm.bestMove);
                }
                canPlay = false;
            }
        }

        private List<Selectable> GetPawns()
        {
            L_Pawns.Clear();
            for (int i = 0; i < GameManager.Instance.player02.childCount; i++)
            {
                Selectable s = GameManager.Instance.player02.GetChild(i).GetComponent<Selectable>();
                L_Pawns.Add(s);
            }
            return L_Pawns;
        }

        private Selectable PickPawn()
        {
            List<Selectable> currentPawns = GetPawns();
            int r = Random.Range(0, currentPawns.Count);
            return currentPawns[r];
        }

        private Selectable PickMovablePawn()
        {
            Selectable s = PickPawn();
            if (s.movablePos.Count > 0)
            {
                return s;
            }
            else
            {
                s = PickMovablePawn();
                return s;
            }
        }

        private List<CapturedCard> GetCapturedCards()
        {
            L_CapturedPawns.Clear();
            List<Selectable> currentPawns = GetPawns();
            foreach (Selectable s in currentPawns)
            {
                if (s.isDead)
                {
                    L_CapturedPawns.Add(s.capturedCard);
                }
            }
            return L_CapturedPawns;
        }

        private bool ChooseMove()
        {
            GetCapturedCards();
            if (L_CapturedPawns.Count <= 0)
            {
                return true;
            }
            else
            {
                int r = Random.Range(0, 2);
                if (r == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}