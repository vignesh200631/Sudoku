
using UnityEngine;

public class DifficultyAnalyzerTester : MonoBehaviour
{
    private void Start()
    {
        TestDifficultyAnalyzer();
    }

    private void TestDifficultyAnalyzer()
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

        DifficultyAnalyzer analyzer =
            new DifficultyAnalyzer();

        DifficultyAnalysisResult result =
            analyzer.Analyze(puzzle);

        Debug.Log("===== DIFFICULTY ANALYSIS =====");

        Debug.Log("Solved: " + result.IsSolved);

        Debug.Log(
            "Highest technique: " +
            result.HighestTechnique
        );

        Debug.Log("Steps used: " + result.StepsUsed);

        Debug.Log(
            "Remaining cells: " +
            result.RemainingCells
        );

        Debug.Log("Status: " + result.Status);

        // Confirm that the original puzzle is unchanged.
        Debug.Log(
            "Original first cell: " +
            puzzle.GetCell(0, 0)
        );
    }
}
