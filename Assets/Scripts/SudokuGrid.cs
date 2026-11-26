using UnityEngine;
using TMPro;

public class SudokuGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int rows = 9;
    [SerializeField] private int columns = 9;
    [SerializeField] private float squareOffset = 0f;
    [SerializeField] private GameObject gridSquare;
    [SerializeField] private float squareScale = 1f;
    [SerializeField] private Vector2 startingPos = Vector2.zero;

    [Header("Highlight Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.75f, 0.85f, 1f);
    [SerializeField] private Color selectedColor = new Color(0.25f, 0.55f, 1f);
    [SerializeField] private Color sameNumberColor = new Color(0.75f, 0.85f, 1f);

    [Header("Validation Colors")]
    [SerializeField] private Color normalNumberColor = Color.black;
    [SerializeField] private Color invalidNumberColor = Color.red;

    [Header("Game Settings")]
    [SerializeField] private int maxMistakes = 3;

    [Header("Level Progress")]
    [SerializeField] private SudokuLevelManager levelManager;

    [Header("UI References")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text mistakeText;

    [Header("Victory UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text completionTimeText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalTimeText;

    private GridSquare[,] gridSquareArray;
    private int[,] puzzle;
    private int[,] solution;
    private int incorrectCount = 0;
    private float elapsedTime = 0f;
    private bool gameOver = false;

    private int selectedRow = -1;
    private int selectedColumn = -1;
    private bool gridInitialized = false;
    private SudokuDifficulty selectedDifficulty = SudokuDifficulty.Easy;

    private void Awake()
    {
        InitializeGridOnce();
    }

    private void InitializeGridOnce()
    {
        if (gridInitialized)
            return;

        if (gridSquare == null)
        {
            return;
        }

        gridSquareArray = new GridSquare[rows, columns];

        CreateGrid();

        gridInitialized = true;
    }

    public void InitializePuzzle(SudokuDifficulty difficulty)
    {
        InitializeGridOnce();

        if (!gridInitialized)
            return;

        // Load selected difficulty.
        selectedDifficulty = difficulty;

        puzzle = SudokuPuzzleData.GetPuzzle(difficulty);
        solution = SudokuPuzzleData.GetSolution(difficulty);

        // Reset every cell using the selected puzzle.
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GridSquare cell = gridSquareArray[row, column];

                cell.SetPuzzleNumber(puzzle[row, column]);
                cell.SetCellColor(normalColor);
            }
        }

        // Reset gameplay state.
        incorrectCount = 0;
        elapsedTime = 0f;
        gameOver = false;

        selectedRow = -1;
        selectedColumn = -1;

        // Reset UI.
        UpdateMistakeCounter();

        timerText.text = "00:00";

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Debug.Log("Loaded " + difficulty + " Sudoku puzzle!");
    }


    public void InitializeLevel(
        SudokuBoard puzzleBoard,
        SudokuBoard solutionBoard)
    {
        InitializeGridOnce();

        if (!gridInitialized)
            return;

        if (puzzleBoard == null || solutionBoard == null)
        {
            Debug.LogError("Invalid Sudoku level data!");
            return;
        }

        puzzle = puzzleBoard.ToTwoDimensionalArray();
        solution = solutionBoard.ToTwoDimensionalArray();

        // Reset every cell using the selected level.
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GridSquare cell = gridSquareArray[row, column];

                cell.SetPuzzleNumber(puzzle[row, column]);
                cell.SetCellColor(normalColor);
            }
        }

        // Reset gameplay state.
        incorrectCount = 0;
        elapsedTime = 0f;
        gameOver = false;

        selectedRow = -1;
        selectedColumn = -1;

        UpdateMistakeCounter();
        timerText.text = "00:00";

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Debug.Log("Sudoku level loaded successfully!");
    }


    private void Update()
    {
        if (gameOver)
            return;

        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void CreateGrid()
    {
        SpawnGridSquare();
        SetGridSquarePosition();
    }

    private void SpawnGridSquare()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject square = Instantiate(gridSquare, transform);

                square.transform.localScale = new Vector3(squareScale, squareScale, squareScale);

                GridSquare cell = square.GetComponent<GridSquare>();

                cell.SetRowAndColumnPos(row, column);
                cell.SetGridManager(this);
                cell.SetNumber(0);
                cell.SetCellColor(normalColor);

                gridSquareArray[row, column] = cell;
            }
        }
    }

    private void SetGridSquarePosition()
    {
        RectTransform squareRect = gridSquare.GetComponent<RectTransform>();

        Vector2 offset = new Vector2( squareRect.rect.width * squareScale + squareOffset, squareRect.rect.height * squareScale + squareOffset);

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector2 position = new Vector2( startingPos.x + offset.x * column, startingPos.y - offset.y * row);

                gridSquareArray[row, column].GetComponent<RectTransform>().anchoredPosition = position;
            }
        }
    }

    // Select a cell and highlight its row, column and block.
    public void SelectCell(int selectedRow, int selectedColumn)
    {
        this.selectedRow = selectedRow;
        this.selectedColumn = selectedColumn;

        int startRow = (selectedRow / 3) * 3;
        int startColumn = (selectedColumn / 3) * 3;

        int selectedNumber = gridSquareArray[selectedRow, selectedColumn].GetNumber();

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GridSquare cell = gridSquareArray[row, column];

                cell.SetCellColor(normalColor);

                bool sameRow = row == selectedRow;
                bool sameColumn = column == selectedColumn;

                bool sameBlock = row >= startRow && row < startRow + 3 && column >= startColumn && column < startColumn + 3;

                if (sameRow || sameColumn || sameBlock)
                {
                    cell.SetCellColor(highlightColor);
                }

                // Highlight cells containing the same number.
                if (selectedNumber != 0 && cell.GetNumber() == selectedNumber)
                {
                    cell.SetCellColor(sameNumberColor);
                }
            }
        }

        gridSquareArray[selectedRow, selectedColumn].SetCellColor(selectedColor);
    }

    // Called when a number button is pressed.
    public void OnNumberButtonClicked(int number)
    {
        if (gameOver)
            return;

        if (selectedRow == -1 || selectedColumn == -1)
        {
            return;
        }

        GridSquare selectedCell = gridSquareArray[selectedRow, selectedColumn];

        if (selectedCell.IsFixed())
        {
            Debug.Log("Cannot change a fixed number!");
            return;
        }

        if (IsCorrectAnswer(selectedRow, selectedColumn, number))
        {
            selectedCell.SetNumber(number);
            selectedCell.SetNumberColor(normalNumberColor);

            CheckWinCondition();
        }
        else
        {
            selectedCell.SetNumber(number);
            selectedCell.SetNumberColor(invalidNumberColor);

            RegisterIncorrectMove();
        }
    }

    public void OnEraseButtonClicked()
    {
        if (selectedRow == -1 || selectedColumn == -1)
        {
            return;
        }

        GridSquare selectedCell = gridSquareArray[selectedRow, selectedColumn];

        if (selectedCell.IsFixed())
        {
            Debug.Log("Cannot erase an original puzzle number!");
            return;
        }

        selectedCell.ClearNumber();

        Debug.Log("Number erased!");
    }

    private void UpdateMistakeCounter()
    {
        mistakeText.text = "Mistakes: " + incorrectCount + "/" + maxMistakes;
    }

    private void RegisterIncorrectMove()
    {
        incorrectCount++;

        UpdateMistakeCounter();

        Debug.Log("Incorrect move: " + incorrectCount);

        if (incorrectCount >= maxMistakes)
        {
            GameOver();
        }
    }

    private bool IsCorrectAnswer(int row, int column, int number)
    {
        return solution[row, column] == number;
    }

    private void GameOver()
    {
        gameOver = true;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        string finalTime = string.Format(
            "{0:00}:{1:00}", minutes, seconds
        );

        finalTimeText.text = "Time: " + finalTime;

        gameOverPanel.SetActive(true);

        Debug.Log("GAME OVER!");
    }

    private void CheckWinCondition()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int playerNumber = gridSquareArray[row, column].GetNumber();

                int correctNumber = solution[row, column];

                if (playerNumber != correctNumber)
                {   
                    return;
                }
            }
        }

        ShowVictoryPanel();
    }

    private void ShowVictoryPanel()
    {
        gameOver = true;

        // Unlock the next level after successful completion.
        if (levelManager != null)
            levelManager.CompleteCurrentLevel();

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        completionTimeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

        victoryPanel.SetActive(true);

        Debug.Log("Congratulations! Sudoku completed!");
    }

    public void RestartPuzzle()
    {
        if (levelManager != null &&
            levelManager.CurrentLevelNumber > 0)
        {
            levelManager.ReloadCurrentLevel();
        }
        else
        {
            InitializePuzzle(selectedDifficulty);
        }
    }

    public void SetDifficulty(SudokuDifficulty difficulty)
    {
        selectedDifficulty = difficulty;
    }

}
