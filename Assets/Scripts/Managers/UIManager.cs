using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerTurn;
    [SerializeField] private TextMeshProUGUI _victoryText;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] public Transform[] capturedPanel;

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
        capturedPanel[0].gameObject.SetActive(false);
        capturedPanel[1].gameObject.SetActive(false);
    }

    public void StaleMate()
    {
        _victoryPanel.SetActive(true);
        _victoryText.text = " Match nul !";
        capturedPanel[0].gameObject.SetActive(false);
        capturedPanel[1].gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        GameManager.Instance.InstanceBoard();
        ClearCapture();
        _victoryPanel.SetActive(false);
        capturedPanel[0].gameObject.SetActive(true);
        capturedPanel[1].gameObject.SetActive(true);
    }

    private void ClearCapture()
    {
        for (int i = 0; i < capturedPanel[0].childCount; i++)
        {
            Destroy(capturedPanel[0].GetChild(i).gameObject);
        }
        for (int i = 0; i < capturedPanel[1].childCount; i++)
        {
            Destroy(capturedPanel[1].GetChild(i).gameObject);
        }
    }
}
