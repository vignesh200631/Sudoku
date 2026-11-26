
using UnityEngine;

public class SudokuDifficultyClassifierTester : MonoBehaviour
{
    [ContextMenu("Test Difficulty Classification")]
    public void TestClassification()
    {
        TestTechnique(SolvingTechnique.NakedSingle, SudokuDifficulty.VeryEasy);
        TestTechnique(SolvingTechnique.HiddenSingle, SudokuDifficulty.Easy);

        TestTechnique(SolvingTechnique.NakedPair, SudokuDifficulty.Medium);
        TestTechnique(SolvingTechnique.HiddenPair, SudokuDifficulty.Medium);

        TestTechnique(SolvingTechnique.PointingPair, SudokuDifficulty.Hard);
        TestTechnique(SolvingTechnique.BoxLineReduction, SudokuDifficulty.Hard);

        TestTechnique(SolvingTechnique.XWing, SudokuDifficulty.Expert);
        TestTechnique(SolvingTechnique.Swordfish, SudokuDifficulty.Master);

        Debug.Log("Difficulty classification tests completed.");
    }

    private void TestTechnique(
        SolvingTechnique technique,
        SudokuDifficulty expected)
    {
        DifficultyAnalysisResult result =
            new DifficultyAnalysisResult(
                true,
                technique,
                1,
                0,
                "Classification test"
            );

        bool classified =
            SudokuDifficultyClassifier.TryClassify(
                result,
                out SudokuDifficulty actual
            );

        if (classified && actual == expected)
        {
            Debug.Log(
                $"PASS: {technique} -> {actual}"
            );
        }
        else
        {
            Debug.LogError(
                $"FAIL: {technique} | Expected: {expected} | Actual: {actual}"
            );
        }
    }
}
