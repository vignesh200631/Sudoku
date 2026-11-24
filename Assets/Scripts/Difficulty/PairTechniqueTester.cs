
#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

public class PairTechniqueTester : MonoBehaviour
{
    private void Start()
    {
        //TestActualNakedPair();
        //TestActualHiddenPair();
        //TestActualPointingPair();
        //TestRowBasedBoxLineReduction();
        //TestColumnBasedBoxLineReduction();
        TestRowBasedXWing();
        TestColumnBasedXWing();
    }

    private void TestActualNakedPair()
    {
        SudokuBoard board = new SudokuBoard();
        CandidateGrid grid = new CandidateGrid(board);

        // Create Naked Pair: {2,7} and {2,7}.
        Restrict(grid, 0, 0, 2, 7);
        Restrict(grid, 0, 1, 2, 7);

        // Target cell: {2,5,7}.
        Restrict(grid, 0, 2, 2, 5, 7);

        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();

        bool detected = analyzer.DebugApplyNakedPair(
            board, grid
        );

        List<int> result = grid.GetCandidates(0, 2);

        bool passed =
            detected &&
            result.Count == 1 &&
            result[0] == 5;

        Debug.Log("===== ACTUAL NAKED PAIR TEST =====");
        Debug.Log("Detected: " + detected);
        Debug.Log("Target candidates: " +
            string.Join(",", result));
        Debug.Log("Expected: 5");
        Debug.Log("TEST PASSED: " + passed);
    }

    private void TestActualHiddenPair()
    {
        SudokuBoard board = new SudokuBoard();
        CandidateGrid grid = new CandidateGrid(board);

        // Two cells contain 2 and 5, plus extra candidates.
        Restrict(grid, 0, 0, 2, 5, 7);
        Restrict(grid, 0, 1, 2, 5, 8);

        // Prevent 2 and 5 from appearing in other
        // cells of row 0.
        for (int column = 2; column < 9; column++)
        {
            for (int number = 1; number <= 9; number++)
            {
                if (number == 2 || number == 5)
                    grid.RemoveCandidate(0, column, number);
            }
        }

        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();

        bool detected = false;

        // Apply repeatedly because the method processes
        // one cell's elimination per call.
        while (analyzer.DebugApplyHiddenPair(board, grid))
        {
            detected = true;
        }

        List<int> cellA = grid.GetCandidates(0, 0);
        List<int> cellB = grid.GetCandidates(0, 1);

        bool passed =
            detected &&
            cellA.Count == 2 &&
            cellA[0] == 2 &&
            cellA[1] == 5 &&
            cellB.Count == 2 &&
            cellB[0] == 2 &&
            cellB[1] == 5;

        Debug.Log("===== ACTUAL HIDDEN PAIR TEST =====");
        Debug.Log("Detected: " + detected);
        Debug.Log("Cell A: " + string.Join(",", cellA));
        Debug.Log("Cell B: " + string.Join(",", cellB));
        Debug.Log("Expected: 2,5 for both cells");
        Debug.Log("TEST PASSED: " + passed);
    }


    private void TestActualPointingPair()
    {
        SudokuBoard board = new SudokuBoard();
        CandidateGrid grid = new CandidateGrid(board);

        // In the top-left block, candidate 4 can appear
        // only at (0,0) and (0,1).
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                if (row == 0 && (column == 0 || column == 1))
                    continue;

                grid.RemoveCandidate(row, column, 4);
            }
        }

        Debug.Log("===== ACTUAL POINTING PAIR TEST =====");

        Debug.Log("Before elimination: " +
            grid.HasCandidate(0, 3, 4));

        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();

        bool detected = false;

        // The analyzer removes one candidate per call.
        while (analyzer.DebugApplyPointingPair(board, grid))
        {
            detected = true;
        }

        bool allRemoved = true;

        //  Check the rest of row 0 outside the block.
        for (int column = 3; column < 9; column++)
        {
            if (grid.HasCandidate(0, column, 4))
            {
                allRemoved = false;
                break;
            }
        }

        Debug.Log("Detected: " + detected);
        Debug.Log("Candidate 4 at (0,3): " +
            grid.HasCandidate(0, 3, 4));

        Debug.Log("All row candidates removed: " + allRemoved);
        Debug.Log("TEST PASSED: " + (detected && allRemoved));
    }


    private void TestRowBasedBoxLineReduction()
    {
        SudokuBoard board = new SudokuBoard();
        CandidateGrid grid = new CandidateGrid(board);

        // Row 3: candidate 7 is possible only
        // in columns 3 and 5.
        for (int column = 0; column < 9; column++)
        {
            if (column != 3 && column != 5)
                grid.RemoveCandidate(3, column, 7);
        }

        Debug.Log("===== ROW BOX-LINE TEST =====");

        Debug.Log("Before elimination at (4,4): " +
            grid.HasCandidate(4, 4, 7));

        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();

        bool detected = false;

        while (analyzer.DebugApplyBoxLineReduction(board, grid))
        {
            detected = true;
        }

        bool allRemoved = true;

        // Check rows 4 and 5 inside the middle block.
        for (int row = 3; row < 6; row++)
        {
            if (row == 3)
                continue;

            for (int column = 3; column < 6; column++)
            {
                if (grid.HasCandidate(row, column, 7))
                    allRemoved = false;
            }
        }

        Debug.Log("Detected: " + detected);
        Debug.Log("Candidate 7 at (4,4): " +
            grid.HasCandidate(4, 4, 7));
        Debug.Log("All block candidates removed: " + allRemoved);
        Debug.Log("TEST PASSED: " + (detected && allRemoved));
    }

    private void TestColumnBasedBoxLineReduction()
    {
        SudokuBoard board = new SudokuBoard();
        CandidateGrid grid = new CandidateGrid(board);

        // Column 4: candidate 6 is possible only
        // in rows 3 and 5.
        for (int row = 0; row < 9; row++)
        {
            if (row != 3 && row != 5)
                grid.RemoveCandidate(row, 4, 6);
        }

        Debug.Log("===== COLUMN BOX-LINE TEST =====");

        Debug.Log("Before elimination at (4,3): " +
            grid.HasCandidate(4, 3, 6));

        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();

        bool detected = false;

        while (analyzer.DebugApplyBoxLineReduction(board, grid))
        {
            detected = true;
        }

        bool allRemoved = true;

        // Check columns 3 and 5 inside the middle block.
        for (int row = 3; row < 6; row++)
        {
            foreach (int column in new int[] { 3, 5 })
            {
                if (grid.HasCandidate(row, column, 6))
                    allRemoved = false;
            }
        }

        Debug.Log("Detected: " + detected);
        Debug.Log("Candidate 6 at (4,3): " +
            grid.HasCandidate(4, 3, 6));
        Debug.Log("All block candidates removed: " + allRemoved);
        Debug.Log("TEST PASSED: " + (detected && allRemoved));
    }



    private void Restrict(
        CandidateGrid grid,
        int row,
        int column,
        params int[] allowed)
    {
        HashSet<int> allowedNumbers =
            new HashSet<int>(allowed);

        for (int number = 1; number <= 9; number++)
        {
            if (!allowedNumbers.Contains(number))
            {
                grid.RemoveCandidate(row, column, number);
            }
        }
    }


    private void TestRowBasedXWing()
    {
        SudokuBoard board = new SudokuBoard();
        CandidateGrid grid = new CandidateGrid(board);

        int number = 1;

        // Row 0 and row 4 contain candidate 1
        // only in columns 1 and 6.
        for (int column = 0; column < 9; column++)
        {
            if (column != 1 && column != 6)
            {
                grid.RemoveCandidate(0, column, number);
                grid.RemoveCandidate(4, column, number);
            }
        }

        Debug.Log("===== ROW-BASED X-WING TEST =====");

        Debug.Log("Before elimination at (2,1): " +
            grid.HasCandidate(2, 1, number));

        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();

        bool detected = false;

        while (analyzer.DebugApplyXWing(board, grid))
        {
            detected = true;
        }

        bool allRemoved = true;

        // Check columns 1 and 6 outside rows 0 and 4.
        for (int row = 0; row < 9; row++)
        {
            if (row == 0 || row == 4)
                continue;

            if (grid.HasCandidate(row, 1, number) ||
                grid.HasCandidate(row, 6, number))
            {
                allRemoved = false;
                break;
            }
        }

        // The four X-Wing corners must remain candidates.
        bool cornersRemain =
            grid.HasCandidate(0, 1, number) &&
            grid.HasCandidate(0, 6, number) &&
            grid.HasCandidate(4, 1, number) &&
            grid.HasCandidate(4, 6, number);

        Debug.Log("Detected: " + detected);
        Debug.Log("All external candidates removed: " + allRemoved);
        Debug.Log("Four corners remain: " + cornersRemain);

        Debug.Log("TEST PASSED: " +
            (detected && allRemoved && cornersRemain));
    }

    private void TestColumnBasedXWing()
    {
        SudokuBoard board = new SudokuBoard();
        CandidateGrid grid = new CandidateGrid(board);

        int number = 2;
 
        // Column 0 and column 5 contain candidate 2
        // only in rows 2 and 7.
        for (int row = 0; row < 9; row++)
        {
            if (row != 2 && row != 7)
            {
                grid.RemoveCandidate(row, 0, number);
                grid.RemoveCandidate(row, 5, number);
            }
        }

        Debug.Log("===== COLUMN-BASED X-WING TEST =====");

        Debug.Log("Before elimination at (2,3): " +
            grid.HasCandidate(2, 3, number));

        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();

        bool detected = false;

        while (analyzer.DebugApplyXWing(board, grid))
        {
            detected = true;
        }

        bool allRemoved = true;

        // Check rows 2 and 7 outside columns 0 and 5.
        for (int column = 0; column < 9; column++)
        {
            if (column == 0 || column == 5)
                continue;

            if (grid.HasCandidate(2, column, number) ||
                grid.HasCandidate(7, column, number))
            {
                allRemoved = false;
                break;
            }
        }

        // The four X-Wing corners must remain candidates.
        bool cornersRemain =
            grid.HasCandidate(2, 0, number) &&
            grid.HasCandidate(2, 5, number) &&
            grid.HasCandidate(7, 0, number) &&
            grid.HasCandidate(7, 5, number);

        Debug.Log("Detected: " + detected);
        Debug.Log("All external candidates removed: " + allRemoved);
        Debug.Log("Four corners remain: " + cornersRemain);

        Debug.Log("TEST PASSED: " +
            (detected && allRemoved && cornersRemain));
    }

}

#endif
