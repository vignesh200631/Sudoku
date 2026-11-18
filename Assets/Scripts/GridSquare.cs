using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridSquare : Selectable, IPointerClickHandler
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private Image cellImage;
    private int number = 0;
    private int row;
    private int col;
    private SudokuGrid sudokuGrid;

    private void DisplayNumber()
    {
        if(number == 0)
            numberText.text = "";
        else
            numberText.text = number.ToString();
    }

    public void SetNumber(int newNumber)
    {
        number = newNumber;
        DisplayNumber();
    }

    public void SetRowAndColumnPos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void SetGridManager(SudokuGrid manager)
    {
        sudokuGrid = manager;
    }

    public void SetCellColor(Color color)
    {
        cellImage.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();

        if (sudokuGrid != null)
        {
            sudokuGrid.SelectCell(row, col);
        }
    }
}
