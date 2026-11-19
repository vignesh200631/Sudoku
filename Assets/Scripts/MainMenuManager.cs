
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private GameObject sudokuPanel;

    [Header("Game Reference")]
    [SerializeField] private SudokuGrid sudokuGrid;

    private void Start()
    {
        ShowHomePanel();
    }

    public void ShowHomePanel()
    {
        homePanel.SetActive(true);
        difficultyPanel.SetActive(false);
        sudokuPanel.SetActive(false);
    }

    public void ShowDifficultyPanel()
    {
        homePanel.SetActive(false);
        difficultyPanel.SetActive(true);
        sudokuPanel.SetActive(false);
    }

    public void StartEasyGame()
    {
        StartGame(SudokuDifficulty.Easy);
    }

    public void StartMediumGame()
    {
        StartGame(SudokuDifficulty.Medium);
    }

    public void StartHardGame()
    {
        StartGame(SudokuDifficulty.Hard);
    }

    private void StartGame(SudokuDifficulty difficulty)
    {
        // Set difficulty before activating SudokuPanel.
        sudokuGrid.SetDifficulty(difficulty);

        homePanel.SetActive(false);
        difficultyPanel.SetActive(false);
        sudokuPanel.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
