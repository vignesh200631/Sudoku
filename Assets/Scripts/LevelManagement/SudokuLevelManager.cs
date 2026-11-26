
using UnityEngine;

public class SudokuLevelManager : MonoBehaviour
{
    [Header("Level Database")]
    [SerializeField] private SudokuLevelDatabase levelDatabase;

    [Header("Gameplay Reference")]
    [SerializeField] private SudokuGrid sudokuGrid;

    private int currentLevelNumber = 0;

    public int CurrentLevelNumber => currentLevelNumber;

    private const string UnlockedLevelKey = "Sudoku_UnlockedLevel";

    private int unlockedLevel = 1;

    public int UnlockedLevel => unlockedLevel;

    public int TotalLevels =>
        levelDatabase != null ? levelDatabase.TotalLevels : 0;


    private void Awake()
    {
        unlockedLevel = PlayerPrefs.GetInt(UnlockedLevelKey, 1);

        if (TotalLevels > 0)
            unlockedLevel = Mathf.Clamp(unlockedLevel, 1, TotalLevels);

        Debug.Log("Unlocked Level: " + unlockedLevel);
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelDatabase == null)
        {
            Debug.LogError("Sudoku Level Database is not assigned!");
            return;
        }

        if (sudokuGrid == null)
        {
            Debug.LogError("Sudoku Grid reference is missing!");
            return;
        }

        SudokuLevelData level =
            levelDatabase.GetLevel(levelNumber);

        if (level == null)
        {
            Debug.LogError("Level " + levelNumber + " not found!");
            return;
        }

        SudokuBoard puzzle = level.GetPuzzle();
        SudokuBoard solution = level.GetSolution();

        sudokuGrid.InitializeLevel(puzzle, solution);

        currentLevelNumber = levelNumber;

        Debug.Log(
            "Loaded Level " + levelNumber +
            " | Difficulty: " + level.Difficulty +
            " | Clues: " + level.ClueCount);
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= unlockedLevel;
    }

    public void CompleteCurrentLevel()
    {
        if (currentLevelNumber <= 0)
            return;

        if (currentLevelNumber >= unlockedLevel &&
            currentLevelNumber < TotalLevels)
        {
            unlockedLevel = currentLevelNumber + 1;

            PlayerPrefs.SetInt(UnlockedLevelKey, unlockedLevel);
            PlayerPrefs.Save();

            Debug.Log("Level " + unlockedLevel + " unlocked!");
        }
    }

    public void ReloadCurrentLevel()
    {
        if (currentLevelNumber <= 0)
        {
            Debug.LogError("No level is currently loaded!");
            return;
        }
    
        LoadLevel(currentLevelNumber);
    }

    [ContextMenu("Test Load Level 98")]
    private void TestLoadLevel1()
    {
        LoadLevel(50);
    }
}
