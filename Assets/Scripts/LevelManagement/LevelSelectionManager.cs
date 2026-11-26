
using UnityEngine;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Level Selection UI")]
    [SerializeField] private Transform content;
    [SerializeField] private LevelButton levelButtonPrefab;
    [SerializeField] private GameObject levelSelectionPanel;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject sudokuPanel;

    [Header("Game References")]
    [SerializeField] private SudokuLevelManager levelManager;



    private void Start()
    {
        GenerateLevelButtons();
    }

    private void GenerateLevelButtons()
    {
        if (content == null || levelButtonPrefab == null)
        {
            Debug.LogError("Level Selection UI references are missing!");
            return;
        }

        if (levelManager == null)
        {
            Debug.LogError("SudokuLevelManager reference is missing!");
            return;
        }

        // Remove previously generated buttons.
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Generate all levels.
        for (int i = 1; i <= levelManager.TotalLevels; i++)
        {
            LevelButton newButton = Instantiate(
                levelButtonPrefab,
                content
            );;

            bool isUnlocked = levelManager.IsLevelUnlocked(i);

            newButton.Setup(
                i,
                isUnlocked,
                SelectLevel
            );
        }

        Debug.Log("Generated " + levelManager.TotalLevels + " level buttons.");
    }

    private void SelectLevel(int levelNumber)
    {
        // Prevent selecting locked levels.
        if (!levelManager.IsLevelUnlocked(levelNumber))
            return;

        levelManager.LoadLevel(levelNumber);

        levelSelectionPanel.SetActive(false);
        sudokuPanel.SetActive(true);

        Debug.Log("Level Selection Active: " + levelSelectionPanel.activeSelf);
        Debug.Log("Sudoku Panel Active: " + sudokuPanel.activeSelf);
    }

    public void RefreshLevelButtons()
    {
        GenerateLevelButtons();
    }
}
