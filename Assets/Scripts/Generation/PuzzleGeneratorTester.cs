
using UnityEngine;

public class PuzzleGeneratorTester : MonoBehaviour
{
    [SerializeField] private int numberOfTests = 10;
    [SerializeField] private int targetClues = 40;

    private void Start()
    {
        TestPuzzleGenerator();
    }

    private void TestPuzzleGenerator()
    {
        SudokuGenerator solutionGenerator =
            new SudokuGenerator();

        PuzzleGenerator puzzleGenerator =
            new PuzzleGenerator();

        int passed = 0;
        int failed = 0;

        for (int i = 0; i < numberOfTests; i++)
        {
            // Generate a completed solution.
            SudokuBoard solution =
                solutionGenerator.Generate();

            // Generate a playable puzzle.
            SudokuPuzzleResult result =
                puzzleGenerator.GeneratePuzzle(
                    solution,
                    targetClues
                );

            // Verify the generated puzzle.
            int solutionCount =
                SudokuSolver.CountSolutions(
                    result.Puzzle, 2
                );

            bool puzzleIsUnique = solutionCount == 1;

            bool solutionIsValid =
                result.Solution.IsCompleteAndValid();

            bool puzzlePreservesSolution = true;

            // Every original clue must match the solution.
            for (int cell = 0;
                 cell < SudokuBoard.CellCount;
                 cell++)
            {
                int puzzleNumber =
                    result.Puzzle.GetCell(cell);

                int solutionNumber =
                    result.Solution.GetCell(cell);

                if (puzzleNumber != 0 &&
                    puzzleNumber != solutionNumber)
                {
                    puzzlePreservesSolution = false;
                    break;
                }
            }

            bool testPassed =
                puzzleIsUnique &&
                solutionIsValid &&
                puzzlePreservesSolution;

            if (testPassed)
            {
                passed++;
            }
            else
            {
                failed++;

                Debug.LogError(
                    "Puzzle test failed: " + (i + 1)
                );
            }

            Debug.Log(
                "Puzzle " + (i + 1) +
                " | Clues: " + result.ClueCount +
                " | Solutions: " + solutionCount +
                " | Valid: " + testPassed
            );

            if (i == 0)
            {
                Debug.Log("FIRST GENERATED PUZZLE");
                PrintBoard(result.Puzzle);

                Debug.Log("CORRECT SOLUTION");
                PrintBoard(result.Solution);
            }
        }

        Debug.Log("===== PUZZLE GENERATOR RESULTS =====");
        Debug.Log("Total tests: " + numberOfTests);
        Debug.Log("Passed: " + passed);
        Debug.Log("Failed: " + failed);

        if (failed == 0)
            Debug.Log("SUCCESS: All puzzles passed validation!");
        else
            Debug.LogError("Some puzzles failed validation.");
    }

    private void PrintBoard(SudokuBoard board)
    {
        for (int row = 0; row < 9; row++)
        {
            string rowText = "";

            for (int column = 0; column < 9; column++)
            {
                int number = board.GetCell(row, column);

                rowText += (number == 0 ? ". " : number + " ");
            }

            Debug.Log(rowText);
        }
    }
}
