
public class DifficultyAnalysisResult
{
    public bool IsSolved { get; private set; }

    public SolvingTechnique HighestTechnique { get; private set; }

    public int StepsUsed { get; private set; }

    public int RemainingCells { get; private set; }

    public string Status { get; private set; }

    public bool HasClassification { get; private set; }

    public SudokuDifficulty ClassifiedDifficulty { get; private set; }

    public SudokuBoard SolvedBoard { get; private set; }

    public DifficultyAnalysisResult(
        bool isSolved,
        SolvingTechnique highestTechnique,
        int stepsUsed,
        int remainingCells,
        string status)
    {
        IsSolved = isSolved;
        HighestTechnique = highestTechnique;
        StepsUsed = stepsUsed;
        RemainingCells = remainingCells;
        Status = status;
    }

    internal void SetClassification(SudokuDifficulty difficulty)
    {
        ClassifiedDifficulty = difficulty;
        HasClassification = true;
    }

    internal void SetSolvedBoard(SudokuBoard board)
    {
        SolvedBoard = board?.Clone();
    }
}
