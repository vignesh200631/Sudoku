
using System;

public static class SudokuSolver
{
    // Find one valid solution.
    public static bool Solve(
        SudokuBoard puzzle,
        out SudokuBoard solution)
    {
        solution = null;

        if (puzzle == null || !IsBoardConsistent(puzzle))
            return false;

        SudokuBoard workingBoard = puzzle.Clone();

        int solutionCount = 0;

        Search(
            workingBoard,
            1,
            ref solutionCount,
            ref solution
        );

        return solutionCount > 0;
    }

    // Count solutions. Stops at the specified limit.
    public static int CountSolutions(
        SudokuBoard puzzle,
        int limit = 2)
    {
        if (puzzle == null)
            return 0;

        if (limit < 1)
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Limit must be at least 1."
            );

        if (!IsBoardConsistent(puzzle))
            return 0;

        SudokuBoard workingBoard = puzzle.Clone();

        int solutionCount = 0;
        SudokuBoard unusedSolution = null;

        Search(
            workingBoard,
            limit,
            ref solutionCount,
            ref unusedSolution
        );

        return solutionCount;
    }

    // Recursive backtracking search.
    private static void Search(
        SudokuBoard board,
        int limit,
        ref int solutionCount,
        ref SudokuBoard firstSolution)
    {
        if (solutionCount >= limit)
            return;

        int row;
        int column;
        int candidateCount;

        FindBestEmptyCell(
            board,
            out row,
            out column,
            out candidateCount
        );

        // No empty cells means the board is solved.
        if (row == -1)
        {
            solutionCount++;

            if (firstSolution == null)
                firstSolution = board.Clone();

            return;
        }

        // No valid candidates means this path is impossible.
        if (candidateCount == 0)
            return;

        // Try each valid candidate.
        for (int number = 1; number <= 9; number++)
        {
            if (!board.CanPlace(row, column, number))
                continue;

            board.SetCell(row, column, number);

            Search(
                board,
                limit,
                ref solutionCount,
                ref firstSolution
            );

            // Backtrack: remove the temporary number.
            board.SetCell(row, column, 0);

            if (solutionCount >= limit)
                return;
        }
    }

    // Find the empty cell with the fewest valid candidates.
    private static void FindBestEmptyCell(
        SudokuBoard board,
        out int bestRow,
        out int bestColumn,
        out int bestCandidateCount)
    {
        bestRow = -1;
        bestColumn = -1;
        bestCandidateCount = 10;

        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (board.GetCell(row, column) != 0)
                    continue;

                int count = 0;

                for (int number = 1; number <= 9; number++)
                {
                    if (board.CanPlace(row, column, number))
                        count++;
                }

                // An empty cell with zero candidates is a dead end.
                if (count == 0)
                {
                    bestRow = row;
                    bestColumn = column;
                    bestCandidateCount = 0;
                    return;
                }

                if (count < bestCandidateCount)
                {
                    bestRow = row;
                    bestColumn = column;
                    bestCandidateCount = count;
                }

                // No better candidate count is possible.
                if (bestCandidateCount == 1)
                    return;
            }
        }
    }

    // Check whether the existing filled cells contain duplicates.
    private static bool IsBoardConsistent(SudokuBoard board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                int number = board.GetCell(row, column);

                if (number == 0)
                    continue;

                if (!board.IsValidPlacement(row, column, number))
                    return false;
            }
        }

        return true;
    }
}
