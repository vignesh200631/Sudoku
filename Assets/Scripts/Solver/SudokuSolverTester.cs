
using UnityEngine;

public class SudokuSolverTester : MonoBehaviour
{
    private void Start()
    {
        TestUniquePuzzle();
        TestInvalidPuzzle();
        TestMultipleSolutions();
    }

    private void TestUniquePuzzle()
    {
        int[] puzzleData =
        {
            5,3,0, 0,7,0, 0,0,0,
            6,0,0, 1,9,5, 0,0,0,
            0,9,8, 0,0,0, 0,6,0,

            8,0,0, 0,6,0, 0,0,3,
            4,0,0, 8,0,3, 0,0,1,
            7,0,0, 0,2,0, 0,0,6,

            0,6,0, 0,0,0, 2,8,0,
            0,0,0, 4,1,9, 0,0,5,
            0,0,0, 0,8,0, 0,7,9
        };

        SudokuBoard puzzle = new SudokuBoard(puzzleData);

        int count = SudokuSolver.CountSolutions(puzzle);

        SudokuBoard solution;
        bool solved = SudokuSolver.Solve(puzzle, out solution);

        Debug.Log("TEST 1 - Unique Puzzle");
        Debug.Log("Solution count: " + count);
        Debug.Log("Solved successfully: " + solved);
        Debug.Log("Solution valid: " +
            (solved && solution.IsCompleteAndValid()));

        // Confirm original puzzle was not modified.
        Debug.Log("Original first cell: " +
            puzzle.GetCell(0, 0));
    }

    private void TestInvalidPuzzle()
    {
        SudokuBoard puzzle = new SudokuBoard();

        // Duplicate 5 in the same row.
        puzzle.SetCell(0, 0, 5);
        puzzle.SetCell(0, 1, 5);

        int count = SudokuSolver.CountSolutions(puzzle);

        Debug.Log("TEST 2 - Invalid Puzzle");
        Debug.Log("Solution count: " + count);
    }

    private void TestMultipleSolutions()
    {
        // An empty board has multiple valid solutions.
        SudokuBoard puzzle = new SudokuBoard();

        int count = SudokuSolver.CountSolutions(puzzle);

        Debug.Log("TEST 3 - Multiple Solutions");
        Debug.Log("Solution count (limit 2): " + count);
    }
}
