
using UnityEngine;

public class SudokuBoardTester : MonoBehaviour
{
    private void Start()
    {
        SudokuBoard board = new SudokuBoard();

        // Test 1: Place a valid number.
        board.SetCell(0, 0, 5);

        Debug.Log(
            "Test 1 - Read cell: " +
            board.GetCell(0, 0)
        );

        // Test 2: Check a valid placement.
        Debug.Log(
            "Test 2 - Can place 3 at (0,1): " +
            board.CanPlace(0, 1, 3)
        );

        // Test 3: Check a duplicate in the same row.
        Debug.Log(
            "Test 3 - Can place 5 at (0,1): " +
            board.CanPlace(0, 1, 5)
        );

        // Test 4: Check a duplicate in the same column.
        board.SetCell(1, 1, 7);

        Debug.Log(
            "Test 4 - Can place 7 at (3,1): " +
            board.CanPlace(3, 1, 7)
        );

        // Test 5: Check a duplicate in the same block.
        Debug.Log(
            "Test 5 - Can place 7 at (2,2): " +
            board.CanPlace(2, 2, 7)
        );

        // Test 6: Test board cloning.
        SudokuBoard copiedBoard = board.Clone();
        copiedBoard.SetCell(0, 0, 9);

        Debug.Log(
            "Test 6 - Original board remains: " +
            board.GetCell(0, 0)
        );
    }
}
