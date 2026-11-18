
using UnityEngine;

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
    [SerializeField] private Color highlightColor = new Color(128f, 230f, 230f);
    [SerializeField] private Color selectedColor = new Color(0.25f, 0.55f, 1f);

    private GridSquare[,] gridSquareArray;

    private void Start()
    {
        if (gridSquare == null)
        {
            Debug.LogError("Grid Square prefab is not assigned!");
            return;
        }

        gridSquareArray = new GridSquare[rows, columns];

        CreateGrid();
        SetGridNumbers();
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
                GameObject square = Instantiate(gridSquare);

                square.transform.SetParent(transform, false);
                square.transform.localScale = new Vector3(squareScale, squareScale, squareScale);

                GridSquare cell = square.GetComponent<GridSquare>();

                cell.SetRowAndColumnPos(row, column);
                cell.SetGridManager(this);

                gridSquareArray[row, column] = cell;
            }
        }
    }

    private void SetGridSquarePosition()
    {
        RectTransform squareRect = gridSquare.GetComponent<RectTransform>();
        float offsetX = squareRect.rect.width * squareScale + squareOffset;
        float offsetY = squareRect.rect.height * squareScale + squareOffset;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                RectTransform cellRect = gridSquareArray[row, column].GetComponent<RectTransform>();
                float posX = startingPos.x + offsetX * column;
                float posY = startingPos.y - offsetY * row;
                cellRect.anchoredPosition = new Vector2(posX, posY);
            }
        }
    }

    private void SetGridNumbers()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                gridSquareArray[row, column].SetNumber(Random.Range(1, 10));
            }
        }
    }


    public void SelectCell(int selectedRow, int selectedColumn)
    {
        int startRow = (selectedRow / 3) * 3;
        int startColumn = (selectedColumn / 3) * 3;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GridSquare cell = gridSquareArray[row, column];

                cell.SetCellColor(normalColor);

                bool isSameRow = row == selectedRow;
                bool isSameColumn = column == selectedColumn;
                bool isSameBlock = row >= startRow && row < startRow + 3 && column >= startColumn && column < startColumn + 3;

                if (isSameRow || isSameColumn || isSameBlock)
                {
                    cell.SetCellColor(highlightColor);
                }
            }
        }

        gridSquareArray[selectedRow, selectedColumn].SetCellColor(selectedColor);
    }

}
