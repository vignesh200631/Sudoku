
public enum SudokuDifficulty
{
    Easy,
    Medium,
    Hard
}

public static class SudokuPuzzleData
{
    // 0 means an empty cell.

    // EASY PUZZLE
    private static readonly int[,] easyPuzzle =
    {
        { 0,0,0,0,0,8,0,0,0 },
        { 0,7,0,1,4,0,3,0,0 },
        { 0,2,4,0,3,0,5,9,1 },
        { 0,0,2,0,0,6,0,7,9 },
        { 3,5,8,7,0,0,0,0,4 },
        { 7,6,0,2,0,4,1,3,0 },
        { 0,1,0,0,7,3,9,5,8 },
        { 0,8,3,9,1,0,7,2,0 },
        { 5,0,0,0,6,0,0,1,0 }
    };

    // EASY SOLUTION
    private static readonly int[,] easySolution =
    {
        { 9,3,1,5,2,8,6,4,7 },
        { 6,7,5,1,4,9,3,8,2 },
        { 8,2,4,6,3,7,5,9,1 },
        { 1,4,2,3,5,6,8,7,9 },
        { 3,5,8,7,9,1,2,6,4 },
        { 7,6,9,2,8,4,1,3,5 },
        { 2,1,6,4,7,3,9,5,8 },
        { 4,8,3,9,1,5,7,2,6 },
        { 5,9,7,8,6,2,4,1,3 }
    };

    // MEDIUM PUZZLE
    private static readonly int[,] mediumPuzzle =
    {
        { 0,0,5,0,0,0,1,0,0 },
        { 0,7,0,5,0,1,3,2,0 },
        { 0,6,0,0,0,0,7,0,0 },
        { 0,0,0,9,4,3,0,0,0 },
        { 8,0,0,2,0,0,0,0,9 },
        { 9,2,4,0,7,0,0,1,0 },
        { 0,0,0,0,0,4,0,0,5 },
        { 5,4,0,1,0,9,0,0,0 },
        { 0,0,6,0,0,2,0,4,1 }
    };

    // MEDIUM SOLUTION
    private static readonly int[,] mediumSolution =
    {
        { 2,3,5,4,6,7,1,9,8 },
        { 4,7,8,5,9,1,3,2,6 },
        { 1,6,9,3,2,8,7,5,4 },
        { 6,1,7,9,4,3,5,8,2 },
        { 8,5,3,2,1,6,4,7,9 },
        { 9,2,4,8,7,5,6,1,3 },
        { 7,9,1,6,8,4,2,3,5 },
        { 5,4,2,1,3,9,8,6,7 },
        { 3,8,6,7,5,2,9,4,1 }
    };

    // HARD PUZZLE
    private static readonly int[,] hardPuzzle =
    {
        { 0,0,6,3,0,7,0,0,0 },
        { 0,0,4,0,0,0,0,0,5 },
        { 1,0,0,0,0,6,0,8,2 },
        { 2,0,5,0,3,0,1,0,6 },
        { 0,0,0,2,0,0,3,0,0 },
        { 9,0,0,0,7,0,0,0,4 },
        { 0,5,0,0,0,0,0,0,0 },
        { 0,1,0,0,0,0,0,0,0 },
        { 0,0,8,1,0,9,0,4,0 }
    };

    // HARD SOLUTION
    private static readonly int[,] hardSolution =
    {
        { 5,8,6,3,2,7,4,9,1 },
        { 7,2,4,8,9,1,6,3,5 },
        { 1,9,3,5,4,6,7,8,2 },
        { 2,4,5,9,3,8,1,7,6 },
        { 8,6,7,2,1,4,3,5,9 },
        { 9,3,1,6,7,5,8,2,4 },
        { 4,5,2,7,6,3,9,1,8 },
        { 3,1,9,4,8,2,5,6,7 },
        { 6,7,8,1,5,9,2,4,3 }
    };

    public static int[,] GetPuzzle(SudokuDifficulty difficulty)
    {
        switch (difficulty)
        {
            case SudokuDifficulty.Easy:
                return easyPuzzle;

            case SudokuDifficulty.Medium:
                return mediumPuzzle;

            case SudokuDifficulty.Hard:
                return hardPuzzle;

            default:
                return easyPuzzle;
        }
    }

    public static int[,] GetSolution(SudokuDifficulty difficulty)
    {
        switch (difficulty)
        {
            case SudokuDifficulty.Easy:
                return easySolution;

            case SudokuDifficulty.Medium:
                return mediumSolution;

            case SudokuDifficulty.Hard:
                return hardSolution;

            default:
                return easySolution;
        }
    }
}
