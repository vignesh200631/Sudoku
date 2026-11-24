
using System;
using System.Collections.Generic;

public class PuzzleGenerator
{
    private readonly Random random;

    public PuzzleGenerator()
    {
        random = new Random();
    }

    public PuzzleGenerator(int seed)
    {
        random = new Random(seed);
    }

    // Generate a puzzle from a completed solution.
    public SudokuPuzzleResult GeneratePuzzle(
        SudokuBoard completedSolution,
        int targetClues)
    {
        if (completedSolution == null)
            throw new ArgumentNullException(
                nameof(completedSolution));

        if (!completedSolution.IsCompleteAndValid())
            throw new ArgumentException(
                "Input must be a complete valid Sudoku solution.");

        if (targetClues < 17 || targetClues > 81)
            throw new ArgumentOutOfRangeException(
                nameof(targetClues),
                "Target clues must be between 17 and 81.");

        // Preserve the original completed solution.
        SudokuBoard solution = completedSolution.Clone();

        // Work on a separate puzzle copy.
        SudokuBoard puzzle = completedSolution.Clone();

        // Create a randomized list of all cell positions.
        List<int> positions = new List<int>();

        for (int i = 0; i < SudokuBoard.CellCount; i++)
        {
            positions.Add(i);
        }

        Shuffle(positions);

        int currentClues = SudokuBoard.CellCount;

        foreach (int index in positions)
        {
            if (currentClues <= targetClues)
                break;

            int originalNumber = puzzle.GetCell(index);

            // Temporarily remove this clue.
            puzzle.SetCell(index, 0);

            // Check whether the puzzle still has exactly one solution.
            int solutionCount =
                SudokuSolver.CountSolutions(puzzle, 2);

            if (solutionCount == 1)
            {
                // Keep the removal.
                currentClues--;
            }
            else
            {
                // Restore the original clue.
                puzzle.SetCell(index, originalNumber);
            }
        }

        return new SudokuPuzzleResult(puzzle, solution);
    }

    // Randomize cell removal order.
    private void Shuffle(List<int> positions)
    {
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(i + 1);

            int temp = positions[i];
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }
    }
}
