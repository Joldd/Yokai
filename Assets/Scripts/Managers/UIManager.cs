using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerTurn;
    [SerializeField] private TextMeshProUGUI _victoryText;
    [SerializeField] private GameObject _victoryPanel;

    public static UIManager Instance { get; private set; }

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
        _victoryPanel.SetActive(false);
    }
    [SerializeField] private Transform[] cardCapture;

    public void UpdatePlayerTurn()
    {
        if (Board.Instance.isPlayer1Turn)
        {
            _playerTurn.text = "Player 1 turn";
        }
        else
        {
            _playerTurn.text = "Player 2 turn";
        }
    }

    public void Victory(int i)
    {
        _victoryPanel.SetActive(true);
        _victoryText.text = " Victoire du joueur " + i + " !";
    }
}
