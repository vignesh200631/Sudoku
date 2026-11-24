
public class DifficultyAnalysisResult
{
    public bool IsSolved { get; private set; }

    public SolvingTechnique HighestTechnique { get; private set; }

    public int StepsUsed { get; private set; }

    public int RemainingCells { get; private set; }

    public string Status { get; private set; }

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
}
