
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SudokuLevelData
{
    [SerializeField] private int levelNumber;
    [SerializeField] private SudokuDifficulty difficulty;
    [SerializeField] private int[] puzzleCells;
    [SerializeField] private int[] solutionCells;
    [SerializeField] private int clueCount;

    public int LevelNumber => levelNumber;
    public SudokuDifficulty Difficulty => difficulty;
    public int ClueCount => clueCount;

    public SudokuLevelData(
        int levelNumber,
        SudokuDifficulty difficulty,
        SudokuPuzzleResult result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));

        this.levelNumber = levelNumber;
        this.difficulty = difficulty;

        puzzleCells = result.Puzzle.ToArray();
        solutionCells = result.Solution.ToArray();
        clueCount = result.ClueCount;
    }

    public SudokuBoard GetPuzzle()
    {
        return new SudokuBoard(puzzleCells);
    }

    public SudokuBoard GetSolution()
    {
        return new SudokuBoard(solutionCells);
    }
}

[CreateAssetMenu(
    fileName = "SudokuLevelDatabase",
    menuName = "Sudoku/Level Database")]
public class SudokuLevelDatabase : ScriptableObject
{
    [SerializeField]
    private List<SudokuLevelData> levels =
        new List<SudokuLevelData>();

    public int TotalLevels => levels.Count;

    public SudokuLevelData GetLevel(int levelNumber)
    {
        return levels.Find(
            level => level.LevelNumber == levelNumber);
    }

    public void SetLevels(List<SudokuLevelData> newLevels)
    {
        if (newLevels == null)
            throw new ArgumentNullException(nameof(newLevels));

        levels = new List<SudokuLevelData>(newLevels);
    }
}
