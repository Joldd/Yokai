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
    [SerializeField] private GameObject _IASelection;
    [SerializeField] private GameObject _modeSelection;

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
        GameManager.Instance.isGameOver = true;
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
        GameManager.Instance.isGameOver = false;
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
        GameManager.Instance.isGameOver = false;
        Board.Instance.isPlayer1Turn = true;
        GameManager.Instance.InstanceBoard();
        GameManager.Instance.isIA = false;
        ClearCapture();
        _victoryPanel.SetActive(false);
        _gamePanel.SetActive(true);
        _menuPanel.SetActive(false);
        GameManager.Instance.gamePaused = false;
    }

    public void SelectIA()
    {
        _IASelection.SetActive(true);
        _modeSelection.SetActive(false);
    }

    public void PlayIA(int depth)
    {
        GameManager.Instance.isGameOver = false;
        Board.Instance.isPlayer1Turn = true;
        GameManager.Instance.isIA = true;
        GameManager.Instance.InstanceBoard();
        ClearCapture();
        _victoryPanel.SetActive(false);
        _gamePanel.SetActive(true);
        _menuPanel.SetActive(false);
        GameManager.Instance.gamePaused = false;
        GameManager.Instance.minimaxAlgorithm.maxDepth = depth;
    }

    public void BackMenu()
    {
        _victoryPanel.SetActive(false);
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
        _pausePanel.SetActive(false);
        _IASelection.SetActive(false);
        _modeSelection.SetActive(true);
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
