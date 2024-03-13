using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class IAManager : MonoBehaviour
{
    public static IAManager Instance { get; private set; }

    private List<Selectable> L_Pawns = new List<Selectable>();

    public bool canPlay;

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
        Board.Instance.changeTurn.AddListener(() =>
        {
            if (!Board.Instance.isPlayer1Turn)
            {
                canPlay = true;
            }
        });
    }

    private void Update()
    {
        if (!Board.Instance.isPlayer1Turn && GameManager.Instance.isIA && canPlay)
        {
            Selectable myPawn = PickMovablePawn();

            int r = Random.Range(0, myPawn.movablePos.Count);
            myPawn.MoveTo(myPawn.movablePos[r]);
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
}
