using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerTurn;
    [SerializeField] private TextMeshProUGUI _victoryText;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] public Transform[] capturedPanel;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _pausePanel;

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
        BackMenu();
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
        _gamePanel.SetActive(false);
        GameManager.Instance.gamePaused = true;
    }

    public void StaleMate()
    {
        _victoryPanel.SetActive(true);
        _victoryText.text = " Match nul !";
        _gamePanel.SetActive(false);
        GameManager.Instance.gamePaused = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        Board.Instance.isPlayer1Turn = true;
        GameManager.Instance.InstanceBoard();
        ClearCapture();
        _victoryPanel.SetActive(false);
        _gamePanel.SetActive(true);
        _pausePanel.SetActive(false);
        GameManager.Instance.gamePaused = false;
    }

    private void ClearCapture()
    {
        for (int i = 0; i < capturedPanel[0].childCount; i++)
        {
            if(capturedPanel[0].GetChild(i).gameObject.GetComponent<Image>() != null)
                Destroy(capturedPanel[0].GetChild(i).gameObject);
        }
        for (int i = 0; i < capturedPanel[1].childCount; i++)
        {
            if (capturedPanel[1].GetChild(i).gameObject.GetComponent<Image>() != null)
                Destroy(capturedPanel[1].GetChild(i).gameObject);
        }
    }

    public void PlayPlayer()
    {
        Board.Instance.isPlayer1Turn = true;
        GameManager.Instance.InstanceBoard();
        ClearCapture();
        _victoryPanel.SetActive(false);
        _gamePanel.SetActive(true);
        _menuPanel.SetActive(false);
        GameManager.Instance.gamePaused = false;
    }

    public void PlayIA()
    {
        Board.Instance.isPlayer1Turn = true;
        GameManager.Instance.InstanceBoard();
        ClearCapture();
        _victoryPanel.SetActive(false);
        _gamePanel.SetActive(true);
        _menuPanel.SetActive(false);
        GameManager.Instance.gamePaused = false;
    }

    public void BackMenu()
    {
        _victoryPanel.SetActive(false);
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
        _pausePanel.SetActive(false);
        GameManager.Instance.gamePaused = true;
    }

    public void Pause()
    {
        _pausePanel.SetActive(true);
        GameManager.Instance.gamePaused = true;
    }

    public void Continue()
    {
        _pausePanel.SetActive(false);
        GameManager.Instance.gamePaused = false;
    }
}
