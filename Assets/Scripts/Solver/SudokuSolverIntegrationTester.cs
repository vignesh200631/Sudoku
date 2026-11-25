
using UnityEngine;

public class SudokuSolverIntegrationTester : MonoBehaviour
{
    [ContextMenu("Run Sudoku Solver Integration Test")]
    public void RunIntegrationTest()
    {
        TestPuzzle(SudokuDifficulty.Hard);
    }

    private void TestPuzzle(SudokuDifficulty difficulty)
    {
        // Get the stored puzzle and its expected solution.
        int[,] puzzleData = SudokuPuzzleData.GetPuzzle(difficulty);
        int[,] solutionData = SudokuPuzzleData.GetSolution(difficulty);

        // Convert both 2D arrays into SudokuBoard objects.
        SudokuBoard puzzleBoard = CreateBoard(puzzleData);
        SudokuBoard solutionBoard = CreateBoard(solutionData);

        // Create the puzzle result.
        SudokuPuzzleResult puzzleResult =
            new SudokuPuzzleResult(puzzleBoard, solutionBoard);

        // Analyze the puzzle using a fresh analyzer.
        DifficultyAnalyzer analyzer = new DifficultyAnalyzer();
        DifficultyAnalysisResult result =
            analyzer.Analyze(puzzleResult.Puzzle);

        // Display the analysis result.
        Debug.Log(
            $"[{difficulty}] Status: {result.Status}\n" +
            $"Solved: {result.IsSolved}\n" +
            $"Highest Technique: {result.HighestTechnique}\n" +
            $"Steps Used: {result.StepsUsed}\n" +
            $"Remaining Cells: {result.RemainingCells}\n" +
            $"Classified Difficulty: " +
            $"{(result.HasClassification ? result.ClassifiedDifficulty.ToString() : "Not Classified")}"
        );

        // Check whether the solver completed the puzzle.

        if (!result.IsSolved)
        {
            Debug.LogWarning(
                $"[{difficulty}] Solver Stuck!\n" +
                $"Highest Technique Used: {result.HighestTechnique}\n" +
                $"Steps Used: {result.StepsUsed}\n" +
                $"Remaining Cells: {result.RemainingCells}\n" +
                $"Status: {result.Status}"
            );

            return;
        }


        // Validate the completed board.
        if (result.SolvedBoard == null ||
            !result.SolvedBoard.IsCompleteAndValid())
        {
            Debug.LogError(
                $"[{difficulty}] FAIL: Solved board is invalid."
            );
            return;
        }

        // Compare the solver's result with the expected solution.
        for (int row = 0; row < SudokuBoard.Size; row++)
        {
            for (int column = 0; column < SudokuBoard.Size; column++)
            {
                if (result.SolvedBoard.GetCell(row, column) !=
                    puzzleResult.Solution.GetCell(row, column))
                {
                    Debug.LogError(
                        $"[{difficulty}] FAIL: Solution mismatch at " +
                        $"Row {row + 1}, Column {column + 1}."
                    );
                    return;
                }
            }
        }

        Debug.Log($"[{difficulty}] PASS: Solution matches the expected solution.");
    }

    private SudokuBoard CreateBoard(int[,] source)
    {
        int[] flatCells = new int[SudokuBoard.CellCount];

        for (int row = 0; row < SudokuBoard.Size; row++)
        {
            for (int column = 0; column < SudokuBoard.Size; column++)
            {
                int index = row * SudokuBoard.Size + column;
                flatCells[index] = source[row, column];
            }
        }

        return new SudokuBoard(flatCells);
    }
}
