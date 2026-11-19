
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

    private bool isFixed = false;
    private SudokuGrid sudokuGrid;

    public void SetGridManager(SudokuGrid manager)
    {
        sudokuGrid = manager;
    }

    public void SetNumber(int newNumber)
    {
        if (isFixed) return;

        number = newNumber;
        DisplayNumber();
        SetNumberColor(Color.black);
    }

    // Used only for original puzzle clues.
    public void SetFixedNumber(int newNumber)
    {
        number = newNumber;
        isFixed = true;

        DisplayNumber();
        SetNumberColor(Color.black);
    }

    public void SetPuzzleNumber(int value)
    {
        number = value;
        isFixed = value != 0;

        DisplayNumber();
        SetNumberColor(Color.black);
    }

    public bool IsFixed()
    {
        return isFixed;
    }

    public int GetNumber()
    {
        return number;
    }

    private void DisplayNumber()
    {
        numberText.text = number == 0 ? "" : number.ToString();
    }

    public void SetNumberColor(Color color)
    {
        numberText.color = color;
    }

    public void SetCellColor(Color color)
    {
        cellImage.color = color;
    }

    public void SetRowAndColumnPos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void ClearNumber()
    {
        if (isFixed) return;

        number = 0;
        DisplayNumber();
        SetNumberColor(Color.black);
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
