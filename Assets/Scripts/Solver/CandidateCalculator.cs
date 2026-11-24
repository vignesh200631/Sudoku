
using System.Collections.Generic;

public static class CandidateCalculator
{
    // Get all valid candidates for one cell.
    public static List<int> GetCandidates(
        SudokuBoard board,
        int row,
        int column)
    {
        List<int> candidates = new List<int>();

        if (board == null)
            return candidates;

        if (board.GetCell(row, column) != 0)
            return candidates;

        for (int number = 1; number <= 9; number++)
        {
            if (board.CanPlace(row, column, number))
            {
                candidates.Add(number);
            }
        }

        return candidates;
    }

    // Get the number of candidates for one cell.
    public static int GetCandidateCount(
        SudokuBoard board,
        int row,
        int column)
    {
        return GetCandidates(board, row, column).Count;
    }
}
