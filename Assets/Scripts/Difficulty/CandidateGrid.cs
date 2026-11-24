
using System.Collections.Generic;

public class CandidateGrid
{
    private readonly bool[,,] candidates =
        new bool[9, 9, 10];

    public CandidateGrid(SudokuBoard board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (board.GetCell(row, column) != 0)
                    continue;

                for (int number = 1; number <= 9; number++)
                {
                    candidates[row, column, number] =
                        board.CanPlace(row, column, number);
                }
            }
        }
    }

    public bool HasCandidate(int row, int column, int number)
    {
        return candidates[row, column, number];
    }

    public void RemoveCandidate(int row, int column, int number)
    {
        candidates[row, column, number] = false;
    }

    public void SetOnlyCandidates(
        int row,
        int column,
        int firstNumber,
        int secondNumber)
    {
        for (int number = 1; number <= 9; number++)
        {
            candidates[row, column, number] =
                number == firstNumber || number == secondNumber;
        }
    }

    public List<int> GetCandidates(int row, int column)
    {
        List<int> result = new List<int>();

        for (int number = 1; number <= 9; number++)
        {
            if (candidates[row, column, number])
                result.Add(number);
        }

        return result;
    }

    public int CountCandidates(int row, int column)
    {
        int count = 0;

        for (int number = 1; number <= 9; number++)
        {
            if (candidates[row, column, number])
                count++;
        }

        return count;
    }
}
