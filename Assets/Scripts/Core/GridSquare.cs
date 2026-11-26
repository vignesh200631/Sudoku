
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridSquare : Selectable, IPointerClickHandler
{
    [Header("Cell UI")]
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private Image cellImage;

    [Header("Pencil Notes")]
    [SerializeField] private TMP_Text[] noteTexts = new TMP_Text[9];

    private int number = 0;
    private int row;
    private int col;

    private bool isFixed = false;
    private bool[] notes = new bool[9];

    private SudokuGrid sudokuGrid;

    public void SetGridManager(SudokuGrid manager)
    {
        sudokuGrid = manager;
    }

    public void SetNumber(int newNumber)
    {
        if (isFixed) return;

        number = newNumber;
        ClearNotes();
        DisplayNumber();
        SetNumberColor(Color.black);
    }

    public void SetFixedNumber(int newNumber)
    {
        number = newNumber;
        isFixed = true;

        ClearNotes();
        DisplayNumber();
        SetNumberColor(Color.black);
    }

    public void SetPuzzleNumber(int value)
    {
        number = value;
        isFixed = value != 0;

        ClearNotes();
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
        ClearNotes();
        DisplayNumber();
        SetNumberColor(Color.black);
    }

    // Add or remove a candidate note.
    public void ToggleNote(int candidate)
    {
        if (isFixed || number != 0)
            return;

        if (candidate < 1 || candidate > 9)
            return;

        int index = candidate - 1;

        notes[index] = !notes[index];

        DisplayNotes();
    }

    // Remove all candidate notes.
    public void ClearNotes()
    {
        for (int i = 0; i < notes.Length; i++)
        {
            notes[i] = false;
        }

        DisplayNotes();
    }

    private void DisplayNotes()
    {
        if (noteTexts == null || noteTexts.Length != 9)
            return;

        for (int i = 0; i < 9; i++)
        {
            if (noteTexts[i] != null)
            {
                noteTexts[i].text = notes[i] ? (i + 1).ToString() : "";
            }
        }
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
