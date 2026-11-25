
public static class SudokuDifficultyClassifier
{
    public static bool TryClassify(DifficultyAnalysisResult result, out SudokuDifficulty difficulty)
    {
        difficulty = default;

        // Only classify puzzles successfully solved by the analyzer.
        if (result == null || !result.IsSolved)
            return false;

        switch (result.HighestTechnique)
        {
            case SolvingTechnique.NakedSingle:
                difficulty = SudokuDifficulty.VeryEasy;
                return true;

            case SolvingTechnique.HiddenSingle:
                difficulty = SudokuDifficulty.Easy;
                return true;

            case SolvingTechnique.NakedPair:
            case SolvingTechnique.HiddenPair:
                difficulty = SudokuDifficulty.Medium;
                return true;

            case SolvingTechnique.PointingPair:
            case SolvingTechnique.BoxLineReduction:
                difficulty = SudokuDifficulty.Hard;
                return true;

            case SolvingTechnique.XWing:
                difficulty = SudokuDifficulty.Expert;
                return true;

            case SolvingTechnique.Swordfish:
                difficulty = SudokuDifficulty.Master;
                return true;

            default:
                return false;
        }
    }
}
