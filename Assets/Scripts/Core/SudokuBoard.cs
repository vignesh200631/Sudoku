
using System;

public class SudokuBoard
{
    public const int Size = 9;
    public const int CellCount = 81;

    private readonly int[] cells;

    // Create an empty Sudoku board.
    public SudokuBoard()
    {
        cells = new int[CellCount];
    }

    // Create a board from existing cell data.
    public SudokuBoard(int[] initialCells)
    {
        if (initialCells == null || initialCells.Length != CellCount)
        {
            throw new ArgumentException("Sudoku board must contain exactly 81 cells.");
        }

        cells = (int[])initialCells.Clone();

        for (int i = 0; i < CellCount; i++)
        {
            if (cells[i] < 0 || cells[i] > 9)
            {
                throw new ArgumentException("Cell values must be between 0 and 9.");
            }
        }
    }

    // Convert row and column into array index.
    public static int GetIndex(int row, int column)
    {
        if (row < 0 || row >= Size || column < 0 || column >= Size)
        {
            throw new ArgumentOutOfRangeException("Invalid Sudoku coordinates.");
        }

        return row * Size + column;
    }

    // Get cell value using row and column.
    public int GetCell(int row, int column)
    {
        return cells[GetIndex(row, column)];
    }

    // Get cell value using array index.
    public int GetCell(int index)
    {
        ValidateIndex(index);
        return cells[index];
    }

    // Set cell value.
    public void SetCell(int row, int column, int value)
    {
        SetCell(GetIndex(row, column), value);
    }

    public void SetCell(int index, int value)
    {
        ValidateIndex(index);

        if (value < 0 || value > 9)
        {
            throw new ArgumentOutOfRangeException("Sudoku values must be between 0 and 9.");
        }

        cells[index] = value;
    }

    // Check whether a number can be placed in a cell.
    public bool CanPlace(int row, int column, int number)
    {
        if (number < 1 || number > 9)
            return false;

        if (GetCell(row, column) != 0)
            return false;

        return IsValidPlacement(row, column, number);
    }

    // Check row, column and 3x3 block rules.
    public bool IsValidPlacement(int row, int column, int number)
    {
        if (row < 0 || row >= Size || column < 0 || column >= Size || number < 1 || number > 9)
        {
            return false;
        }

        // Check row.
        for (int col = 0; col < Size; col++)
        {
            if (col != column && GetCell(row, col) == number)
            {
                return false;
            }
        }

        // Check column.
        for (int r = 0; r < Size; r++)
        {
            if (r != row && GetCell(r, column) == number)
            {
                return false;
            }
        }

        // Find starting position of 3x3 block.
        int startRow = (row / 3) * 3;
        int startColumn = (column / 3) * 3;

        // Check 3x3 block.
        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startColumn; c < startColumn + 3; c++)
            {
                if ((r != row || c != column) && GetCell(r, c) == number)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Check whether the complete board is valid.
    public bool IsCompleteAndValid()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                int number = GetCell(row, column);

                if (number == 0)
                    return false;

                if (!IsValidPlacement(row, column, number))
                    return false;
            }
        }

        return true;
    }

    // Create an independent copy of this board.
    public SudokuBoard Clone()
    {
        return new SudokuBoard(cells);
    }

    // Return a copy of all 81 values.
    public int[] ToArray()
    {
        return (int[])cells.Clone();
    }

    // Convert board into a 2D array for existing UI.
    public int[,] ToTwoDimensionalArray()
    {
        int[,] result = new int[Size, Size];

        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                result[row, column] = GetCell(row, column);
            }
        }

        return result;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= CellCount)
        {
            throw new ArgumentOutOfRangeException("Invalid Sudoku cell index.");
        }
    }
}
