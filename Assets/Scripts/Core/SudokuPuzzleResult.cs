
public class SudokuPuzzleResult
{
    public SudokuBoard Puzzle { get; private set; }
    public SudokuBoard Solution { get; private set; }
    public int ClueCount { get; private set; }

    public SudokuPuzzleResult(
        SudokuBoard puzzle,
        SudokuBoard solution)
    {
        Puzzle = puzzle.Clone();
        Solution = solution.Clone();

        int count = 0;

        for (int i = 0; i < SudokuBoard.CellCount; i++)
        {
            if (Puzzle.GetCell(i) != 0)
                count++;
        }

        ClueCount = count;
    }
}
