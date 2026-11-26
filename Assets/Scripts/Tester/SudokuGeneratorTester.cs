
using System.Collections.Generic;
using UnityEngine;

public class SudokuGeneratorTester : MonoBehaviour
{
    [SerializeField] private int numberOfTests = 100;

    private void Start()
    {
        TestGenerator();
    }

    private void TestGenerator()
    {
        SudokuGenerator generator = new SudokuGenerator();

        int passed = 0;
        int failed = 0;

        HashSet<string> uniqueBoards = new HashSet<string>();

        for (int i = 0; i < numberOfTests; i++)
        {
            SudokuBoard board = generator.Generate();

            // Test 1: All 81 cells are filled and valid.
            bool isValid = board.IsCompleteAndValid();

            // Test 2: A completed valid board has exactly one solution.
            int solutionCount = SudokuSolver.CountSolutions(board);

            if (isValid && solutionCount == 1)
            {
                passed++;
            }
            else
            {
                failed++;

                Debug.LogError(
                    "Generation test failed at board: " + (i + 1)
                );
            }

            // Check whether this board is different from previous boards.
            string boardKey = string.Join("", board.ToArray());
            uniqueBoards.Add(boardKey);

            // Display the first generated board.
            if (i == 0)
            {
                Debug.Log("FIRST GENERATED SUDOKU BOARD");
                PrintBoard(board);
            }
        }

        Debug.Log("===== GENERATOR TEST RESULTS =====");
        Debug.Log("Total tests: " + numberOfTests);
        Debug.Log("Passed: " + passed);
        Debug.Log("Failed: " + failed);
        Debug.Log("Unique boards: " + uniqueBoards.Count);

        if (failed == 0)
        {
            Debug.Log("SUCCESS: All generated boards are valid!");
        }
        else
        {
            Debug.LogError("Some generated boards failed validation.");
        }
    }

    private void PrintBoard(SudokuBoard board)
    {
        for (int row = 0; row < 9; row++)
        {
            string rowText = "";

            for (int column = 0; column < 9; column++)
            {
                rowText += board.GetCell(row, column) + " ";
            }

            Debug.Log(rowText);
        }
    }
}
