using System.Collections.Generic;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;

namespace Groupe10
{
    public class IAManager : MonoBehaviour, ICompetitor
    {
        public static IAManager Instance { get; private set; }

        private List<Selectable> L_Pawns = new List<Selectable>();
        private List<CapturedCard> L_CapturedPawns = new List<CapturedCard>();

        public bool canPlay;

        private MinimaxAlgorithm minimaxAlgorithm;

        public string nameCompetitor;

        private ECampType camp;
        public IGameManager myGameManager;
        private float timeForPlay;

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
                    //GameManager.Instance.GetPawn(minimaxAlgorithm.bestPawn.currentPos).MoveTo(minimaxAlgorithm.bestMove);
                    myGameManager.DoAction(GetIPawnFromTempPawn(minimaxAlgorithm.bestPawn, false), minimaxAlgorithm.bestMove, EActionType.MOVE);
                }
                else if (!minimaxAlgorithm.IAmove && minimaxAlgorithm.IAparachute)
                {
                    myGameManager.DoAction(GetIPawnFromTempPawn(minimaxAlgorithm.bestPawn, true), minimaxAlgorithm.bestMove, EActionType.PARACHUTE);
                    //GameManager.Instance.GetCapturedCard(minimaxAlgorithm.bestPawn).InvokePiece(minimaxAlgorithm.bestMove);
                }
                canPlay = false;
            }
        }

        private EPawnType GetEPawnTypeFromType(PawnType type)
        {
            switch (type)
            {
                case PawnType.KODAMA:
                    return EPawnType.Kodama;
                case PawnType.KITSUNE:
                    return EPawnType.Kitsune;
                case PawnType.KOROPOKKURU:
                    return EPawnType.Koropokkuru;
                case PawnType.SAMURAI:
                    return EPawnType.KodamaSamurai;
                case PawnType.TANUKI:
                    return EPawnType.Tanuki;
                default:
                    return EPawnType.Kodama;
            }
        }

        private IPawn GetIPawnFromTempPawn(TempPawn tempPawn, bool isDead)
        {
            if (!isDead)
            {
                foreach (IPawn ipawn in myGameManager.GetPawnsOnBoard(camp))
                {
                    if (ipawn.GetCurrentPosition() == tempPawn.currentPos)
                    {
                        return ipawn;
                    }
                }
            }
            if (isDead)
            {
                foreach (IPawn ipawn in myGameManager.GetReservePawnsByPlayer(camp))
                {
                    if (ipawn.GetPawnType() == GetEPawnTypeFromType(tempPawn.pawnType))
                    {
                        return ipawn;
                    }
                }
            }
            return null;
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

        //////////////////////// INTERFACE /////////////////////////////////////////////////////

        public void Init(IGameManager igameManager, float timerForAI, ECampType currentCamp)
        {
            myGameManager = igameManager;
            camp = currentCamp;
            timeForPlay = timerForAI;
        }

        public string GetName()
        {
            return nameCompetitor;
        }

        public ECampType GetCamp()
        {
            return camp;
        }

        public void GetDatas()
        {
            throw new System.NotImplementedException();
        }

        public void StartTurn()
        {
            canPlay = true;
        }

        public void StopTurn()
        {
            canPlay = false;
        }
    }
}