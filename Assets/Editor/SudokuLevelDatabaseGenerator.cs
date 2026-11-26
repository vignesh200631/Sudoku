
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SudokuLevelDatabaseGenerator
{
    private const int TotalLevels = 100;
    private const int MaxAttemptsPerLevel = 10000;

    [MenuItem("Tools/Sudoku/Generate 100 Levels")]
    public static void GenerateLevels()
    {
        SudokuLevelDatabase database =
            Selection.activeObject as SudokuLevelDatabase;

        if (database == null)
        {
            Debug.LogError("Select your SudokuLevels asset first.");
            return;
        }

        // Load previously generated levels.
        List<SudokuLevelData> generatedLevels =
            new List<SudokuLevelData>();

        for (int i = 1; i <= TotalLevels; i++)
        {
            SudokuLevelData existingLevel = database.GetLevel(i);

            if (existingLevel != null)
                generatedLevels.Add(existingLevel);
        }

        SudokuGenerator solutionGenerator =
            new SudokuGenerator();

        PuzzleGenerator puzzleGenerator =
            new PuzzleGenerator();

        DifficultyAnalyzer analyzer =
            new DifficultyAnalyzer();

        try
        {
            for (int level = 1; level <= TotalLevels; level++)
            {
                // Skip levels already saved.
                if (generatedLevels.Exists(
                    item => item.LevelNumber == level))
                {
                    Debug.Log($"Level {level} already exists. Skipping.");
                    continue;
                }

                SudokuDifficulty targetDifficulty =
                    GetTargetDifficulty(level);

                bool success = false;

                for (int attempt = 1;
                     attempt <= MaxAttemptsPerLevel;
                     attempt++)
                {
                    float progress =
                        (float)(level - 1) / TotalLevels;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Generating Sudoku Levels",
                        $"Level {level}/100 - {targetDifficulty}\n" +
                        $"Attempt {attempt}/{MaxAttemptsPerLevel}",
                        progress))
                    {
                        Debug.LogWarning(
                            "Generation cancelled. " +
                            "Previously saved levels are preserved.");

                        return;
                    }

                    SudokuBoard solution =
                        solutionGenerator.Generate();

                    int targetClues = Random.Range(
                        GetMinimumClues(targetDifficulty),
                        GetMaximumClues(targetDifficulty) + 1);

                    SudokuPuzzleResult puzzleResult =
                        puzzleGenerator.GeneratePuzzle(
                            solution,
                            targetClues);

                    DifficultyAnalysisResult analysis =
                        analyzer.Analyze(puzzleResult.Puzzle);

                    if (!analysis.IsSolved ||
                        !analysis.HasClassification ||
                        analysis.ClassifiedDifficulty != targetDifficulty)
                    {
                        continue;
                    }

                    generatedLevels.Add(
                        new SudokuLevelData(
                            level,
                            targetDifficulty,
                            puzzleResult));

                    // Keep levels ordered by level number.
                    generatedLevels.Sort(
                        (a, b) => a.LevelNumber.CompareTo(b.LevelNumber));

                    // Save immediately after every successful level.
                    Undo.RecordObject(
                        database,
                        "Generate Sudoku Level");

                    database.SetLevels(generatedLevels);

                    EditorUtility.SetDirty(database);
                    AssetDatabase.SaveAssets();

                    success = true;

                    Debug.Log(
                        $"Level {level} generated and saved. " +
                        $"Difficulty: {targetDifficulty}, " +
                        $"Clues: {puzzleResult.ClueCount}, " +
                        $"Attempts: {attempt}");

                    break;
                }

                if (!success)
                {
                    Debug.LogError(
                        $"Could not generate Level {level} " +
                        $"({targetDifficulty}). " +
                        $"Saved levels: {generatedLevels.Count}. " +
                        "Run the generator again to resume.");

                    return;
                }
            }

            Debug.Log(
                $"Successfully generated all {TotalLevels} levels!");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static SudokuDifficulty GetTargetDifficulty(int level)
    {
        if (level <= 10) return SudokuDifficulty.VeryEasy;
        if (level <= 25) return SudokuDifficulty.Easy;
        if (level <= 45) return SudokuDifficulty.Medium;
        if (level <= 65) return SudokuDifficulty.Hard;
        if (level <= 85) return SudokuDifficulty.Expert;

        return SudokuDifficulty.Master;
    }

    private static int GetMinimumClues(SudokuDifficulty difficulty)
    {
        switch (difficulty)
        {
            case SudokuDifficulty.VeryEasy: return 42;
            case SudokuDifficulty.Easy: return 36;
            case SudokuDifficulty.Medium: return 30;
            case SudokuDifficulty.Hard: return 26;
            case SudokuDifficulty.Expert: return 23;
            case SudokuDifficulty.Master: return 22;
            default: return 30;
        }
    }

    private static int GetMaximumClues(SudokuDifficulty difficulty)
    {
        switch (difficulty)
        {
            case SudokuDifficulty.VeryEasy: return 50;
            case SudokuDifficulty.Easy: return 44;
            case SudokuDifficulty.Medium: return 38;
            case SudokuDifficulty.Hard: return 34;
            case SudokuDifficulty.Expert: return 30;
            case SudokuDifficulty.Master: return 28;
            default: return 40;
        }
    }
}
