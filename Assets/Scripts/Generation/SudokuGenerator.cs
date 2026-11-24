
using System;
using System.Collections.Generic;

public class SudokuGenerator
{
    private readonly Random random;

    // Generate with a random seed.
    public SudokuGenerator()
    {
        random = new Random();
    }

    // Generate with a specific seed for reproducible results.
    public SudokuGenerator(int seed)
    {
        random = new Random(seed);
    }

    // Generate a complete Sudoku solution.
    public SudokuBoard Generate()
    {
        SudokuBoard board = new SudokuBoard();

        bool generated = FillBoard(board);

        if (!generated || !board.IsCompleteAndValid())
        {
            throw new InvalidOperationException(
                "Failed to generate a valid Sudoku board."
            );
        }

        return board;
    }

    // Recursively fill the board using randomized backtracking.
    private bool FillBoard(SudokuBoard board)
    {
        int selectedRow;
        int selectedColumn;
        List<int> candidates;

        if (!FindBestCell(
            board,
            out selectedRow,
            out selectedColumn,
            out candidates))
        {
            // No empty cells remain.
            return true;
        }

        // Try candidates in random order.
        Shuffle(candidates);

        foreach (int number in candidates)
        {
            board.SetCell(selectedRow, selectedColumn, number);

            if (FillBoard(board))
                return true;

            // Backtrack if this choice leads to a dead end.
            board.SetCell(selectedRow, selectedColumn, 0);
        }

        return false;
    }

    // Find an empty cell with the fewest valid candidates.
    private bool FindBestCell( SudokuBoard board, out int bestRow, out int bestColumn, out List<int> bestCandidates)
    {
        bestRow = -1;
        bestColumn = -1;
        bestCandidates = null;

        int minimumCount = 10;

        // Store cells tied for the fewest candidates.
        List<int[]> bestCells = new List<int[]>();

        for (int row = 0; row < SudokuBoard.Size; row++)
        {
            for (int column = 0;
                 column < SudokuBoard.Size; column++)
            {
                if (board.GetCell(row, column) != 0)
                    continue;

                List<int> candidates = GetCandidates(
                    board,
                    row,
                    column
                );

                int count = candidates.Count;

                if (count == 0)
                {
                    // Return this empty cell with no candidates.
                    // FillBoard will try nothing and backtrack.
                    bestRow = row;
                    bestColumn = column;
                    bestCandidates = candidates;

                    return true;
                }

                if (count < minimumCount)
                {
                    minimumCount = count;

                    bestCells.Clear();
                    bestCells.Add(new int[] { row, column });
                }
                else if (count == minimumCount)
                {
                    bestCells.Add(new int[] { row, column });
                }
            }
        }

        // No empty cells means the board is complete.
        if (bestCells.Count == 0)
            return false;

        // Randomly choose between equally suitable cells.
        int selectedIndex = random.Next(bestCells.Count);

        bestRow = bestCells[selectedIndex][0];
        bestColumn = bestCells[selectedIndex][1];

        bestCandidates = GetCandidates(
            board,
            bestRow,
            bestColumn
        );

        return true;
    }

    // Calculate all valid numbers for a cell.
    private List<int> GetCandidates(
        SudokuBoard board,
        int row,
        int column)
    {
        List<int> candidates = new List<int>();

        for (int number = 1; number <= 9; number++)
        {
            if (board.CanPlace(row, column, number))
            {
                candidates.Add(number);
            }
        }

        return candidates;
    }

    // Fisher-Yates shuffle.
    private void Shuffle(List<int> values)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(i + 1);

            int temp = values[i];
            values[i] = values[randomIndex];
            values[randomIndex] = temp;
        }
    }
}
