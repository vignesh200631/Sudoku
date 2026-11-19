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

    [Header("UI References")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text mistakeText;

    [Header("Victory UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text completionTimeText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalTimeText;

    private int incorrectCount = 0;
    private float elapsedTime = 0f;
    private bool gameOver = false;

    private GridSquare[,] gridSquareArray;

    private int selectedRow = -1;
    private int selectedColumn = -1;

    private int[,] puzzle;

    private int[,] solution;

    private SudokuDifficulty selectedDifficulty = SudokuDifficulty.Easy;

    private void Start()
    {
        if (gridSquare == null)
        {
            Debug.LogError("Grid Square is not assigned!");
            return;
        }

        // Load selected puzzle and its solution.
        puzzle = SudokuPuzzleData.GetPuzzle(selectedDifficulty);
        solution = SudokuPuzzleData.GetSolution(selectedDifficulty);

        gridSquareArray = new GridSquare[rows, columns];

        CreateGrid();
        LoadPuzzle();

        incorrectCount = 0;
        elapsedTime = 0f;
        gameOver = false;

        UpdateMistakeCounter();

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
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

                square.transform.localScale =
                    new Vector3(squareScale, squareScale, squareScale);

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
        RectTransform squareRect =
            gridSquare.GetComponent<RectTransform>();

        Vector2 offset = new Vector2(
            squareRect.rect.width * squareScale + squareOffset,
            squareRect.rect.height * squareScale + squareOffset
        );

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector2 position = new Vector2(
                    startingPos.x + offset.x * column,
                    startingPos.y - offset.y * row
                );

                gridSquareArray[row, column]
                    .GetComponent<RectTransform>().anchoredPosition = position;
            }
        }
    }

    private void LoadPuzzle()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int value = puzzle[row, column];

                if (value != 0)
                {
                    gridSquareArray[row, column].SetFixedNumber(value);
                }
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

                bool sameBlock =
                    row >= startRow && row < startRow + 3 &&
                    column >= startColumn && column < startColumn + 3;

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

        gridSquareArray[selectedRow, selectedColumn]
            .SetCellColor(selectedColor);
    }

    // Called when a number button is pressed.
    public void OnNumberButtonClicked(int number)
    {
        if (gameOver)
            return;

        if (selectedRow == -1 || selectedColumn == -1)
        {
            Debug.Log("Please select a cell first.");
            return;
        }

        GridSquare selectedCell =
            gridSquareArray[selectedRow, selectedColumn];

        if (selectedCell.IsFixed())
        {
            Debug.Log("Cannot change a fixed number!");
            return;
        }

        if (IsCorrectAnswer(selectedRow, selectedColumn, number))
        {
            selectedCell.SetNumber(number);
            selectedCell.SetNumberColor(normalNumberColor);

            Debug.Log("Correct answer!");

            CheckWinCondition();
        }
        else
        {
            selectedCell.SetNumber(number);
            selectedCell.SetNumberColor(invalidNumberColor);

            RegisterIncorrectMove();

            Debug.Log("Wrong answer!");
        }
    }

    public void OnEraseButtonClicked()
    {
        if (selectedRow == -1 || selectedColumn == -1)
        {
            Debug.Log("Please select a cell first.");
            return;
        }

        GridSquare selectedCell =
            gridSquareArray[selectedRow, selectedColumn];

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
        mistakeText.text = "Mistakes: " +
            incorrectCount + "/" + maxMistakes;
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
                int playerNumber =
                    gridSquareArray[row, column].GetNumber();

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

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        completionTimeText.text =
            string.Format("Time: {0:00}:{1:00}", minutes, seconds);

        victoryPanel.SetActive(true);

        Debug.Log("Congratulations! Sudoku completed!");
    }


    public void RestartPuzzle()
    {
        // Reset game status.
        gameOver = false;

        // Reset timer and mistakes.
        elapsedTime = 0f;
        incorrectCount = 0;

        // Update UI.
        UpdateMistakeCounter();
        timerText.text = "00:00";

        // Hide victory panel.
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        //Hide gameover panel
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Reset every cell to the original puzzle.
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int originalNumber = puzzle[row, column];

                GridSquare cell = gridSquareArray[row, column];

                if (originalNumber != 0)
                {
                    cell.SetFixedNumber(originalNumber);
                }
                else
                {
                    cell.SetNumber(0);
                    cell.SetNumberColor(normalNumberColor);
                }

                cell.SetCellColor(normalColor);
            }
        }

        // Clear selected cell.
        selectedRow = -1;
        selectedColumn = -1;

        Debug.Log("Puzzle restarted!");
    }

    public void SetDifficulty(SudokuDifficulty difficulty)
    {
        selectedDifficulty = difficulty;
    }

}
