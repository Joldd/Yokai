using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerTurn;

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
}
