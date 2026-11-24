
using System.Collections.Generic;

public class DifficultyAnalyzer
{
    private SudokuBoard board;
    private CandidateGrid candidateGrid;

    private int steps;
    private SolvingTechnique highestTechnique;

    public DifficultyAnalysisResult Analyze(SudokuBoard originalPuzzle)
    {
        if (originalPuzzle == null)
        {
            return new DifficultyAnalysisResult(
                false, SolvingTechnique.None,
                0, 81, "Invalid puzzle."
            );
        }

        board = originalPuzzle.Clone();

        if (!IsBoardConsistent(board))
        {
            return new DifficultyAnalysisResult(
                false, SolvingTechnique.None,
                0, CountEmptyCells(board),
                "Puzzle contains duplicate numbers."
            );
        }

        candidateGrid = new CandidateGrid(board);
        steps = 0;
        highestTechnique = SolvingTechnique.None;

        while (true)
        {
            if (board.IsCompleteAndValid())
                return CreateResult(true, "Solved using logical techniques.");

            if (ApplyNakedSingle())
                continue;

            if (ApplyHiddenSingle())
                continue;

            if (ApplyNakedPair())
                continue;

            if (ApplyHiddenPair())
                continue;

            if(ApplyPointingPair())
                continue;

            if (ApplyBoxLineReduction())
                continue;

            if (ApplyXWing())
                continue;

            break;
        }

        return CreateResult(
            false,
            "Stuck: no supported logical technique found."
        );
    }

    private bool ApplyNakedSingle()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                if (board.GetCell(row, column) != 0)
                    continue;

                List<int> candidates =
                    candidateGrid.GetCandidates(row, column);

                if (candidates.Count == 1)
                {
                    PlaceNumber(
                        row, column, candidates[0],
                        SolvingTechnique.NakedSingle
                    );

                    return true;
                }
            }
        }

        return false;
    }

    private bool ApplyHiddenSingle()
    {
        // Check rows.
        for (int row = 0; row < 9; row++)
        {
            for (int number = 1; number <= 9; number++)
            {
                int foundColumn = -1;
                int count = 0;

                for (int column = 0; column < 9; column++)
                {
                    if (candidateGrid.HasCandidate(
                        row, column, number))
                    {
                        count++;
                        foundColumn = column;
                    }
                }

                if (count == 1)
                {
                    PlaceNumber(
                        row, foundColumn, number,
                        SolvingTechnique.HiddenSingle
                    );

                    return true;
                }
            }
        }

        // Check columns.
        for (int column = 0; column < 9; column++)
        {
            for (int number = 1; number <= 9; number++)
            {
                int foundRow = -1;
                int count = 0;

                for (int row = 0; row < 9; row++)
                {
                    if (candidateGrid.HasCandidate(
                        row, column, number))
                    {
                        count++;
                        foundRow = row;
                    }
                }

                if (count == 1)
                {
                    PlaceNumber(
                        foundRow, column, number,
                        SolvingTechnique.HiddenSingle
                    );

                    return true;
                }
            }
        }

        // Check 3x3 blocks.
        for (int blockRow = 0; blockRow < 3; blockRow++)
        {
            for (int blockColumn = 0; blockColumn < 3; blockColumn++)
            {
                for (int number = 1; number <= 9; number++)
                {
                    int foundRow = -1;
                    int foundColumn = -1;
                    int count = 0;

                    for (int row = blockRow * 3;
                         row < blockRow * 3 + 3; row++)
                    {
                        for (int column = blockColumn * 3;
                             column < blockColumn * 3 + 3; column++)
                        {
                            if (candidateGrid.HasCandidate(
                                row, column, number))
                            {
                                count++;
                                foundRow = row;
                                foundColumn = column;
                            }
                        }
                    }

                    if (count == 1)
                    {
                        PlaceNumber(
                            foundRow, foundColumn, number,
                            SolvingTechnique.HiddenSingle
                        );

                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Naked Pair: two cells have exactly the same two candidates.
    
    private bool ApplyNakedPair()
    {
        for (int unit = 0; unit < 27; unit++)
        {
            List<int[]> cells = GetUnitCells(unit);

            for (int i = 0; i < cells.Count; i++)
            {
                int rowA = cells[i][0];
                int colA = cells[i][1];

                if (board.GetCell(rowA, colA) != 0)
                    continue;

                List<int> pair =
                    candidateGrid.GetCandidates(rowA, colA);

                if (pair.Count != 2)
                    continue;

                List<int[]> matchingCells = new List<int[]>();

                for (int j = 0; j < cells.Count; j++)
                {
                    int row = cells[j][0];
                    int column = cells[j][1];

                    if (board.GetCell(row, column) != 0)
                        continue;

                    List<int> other =
                        candidateGrid.GetCandidates(row, column);

                    if (SamePair(pair, other))
                        matchingCells.Add(cells[j]);
                }

                // A valid Naked Pair must occupy exactly two cells.
                if (matchingCells.Count != 2 ||
                    !SameCell(matchingCells[0], cells[i]))
                    continue;

                foreach (int[] cell in cells)
                {
                    if (SameCell(cell, matchingCells[0]) ||
                        SameCell(cell, matchingCells[1]))
                        continue;

                    int row = cell[0];
                    int column = cell[1];

                    if (board.GetCell(row, column) != 0)
                        continue;

                    bool changed = false;

                    foreach (int number in pair)
                    {
                        if (candidateGrid.HasCandidate(
                            row, column, number))
                        {
                            candidateGrid.RemoveCandidate(
                                row, column, number);

                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        steps++;
                        RecordTechnique(SolvingTechnique.NakedPair);
                        return true;
                    }
                }
            }
        }

        return false;
    }   


    // Hidden Pair: two numbers occur in exactly the same two cells.
    private bool ApplyHiddenPair()
    {
        for (int unit = 0; unit < 27; unit++)
        {
            List<int[]> cells = GetUnitCells(unit);

            for (int first = 1; first <= 9; first++)
            {
                for (int second = first + 1; second <= 9; second++)
                {
                    List<int[]> firstPositions =
                        GetCandidatePositions(cells, first);

                    if (firstPositions.Count != 2)
                        continue;

                    List<int[]> secondPositions =
                        GetCandidatePositions(cells, second);

                    if (secondPositions.Count != 2)
                        continue;

                    if (!SamePositions(
                        firstPositions, secondPositions))
                        continue;

                    // Restrict the two cells to this pair.
                    foreach (int[] position in firstPositions)
                    {
                        int row = position[0];
                        int column = position[1];

                        List<int> existing =
                            candidateGrid.GetCandidates(row, column);

                        bool changed = false;

                        foreach (int candidate in existing)
                        {
                            if (candidate != first &&
                                candidate != second)
                            {
                                candidateGrid.RemoveCandidate(
                                    row, column, candidate
                                );

                                changed = true;
                            }
                        }

                        if (changed)
                        {
                            RecordTechnique(SolvingTechnique.HiddenPair);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    private bool ApplyPointingPair()
    {
        // Inspect all nine 3x3 blocks.
        for (int blockRow = 0; blockRow < 3; blockRow++)
        {
            for (int blockColumn = 0; blockColumn < 3; blockColumn++)
            {
                int startRow = blockRow * 3;
                int startColumn = blockColumn * 3;

                for (int number = 1; number <= 9; number++)
                {
                    List<int[]> positions = new List<int[]>();

                    // Find candidate positions inside this block.
                    for (int row = startRow; row < startRow + 3; row++)
                    {
                        for (int column = startColumn;
                             column < startColumn + 3; column++)
                        {
                            if (board.GetCell(row, column) == 0 &&
                                candidateGrid.HasCandidate(row, column, number))
                            {
                                positions.Add(new int[] { row, column });
                            }
                        }
                    }

                    // Need at least two positions.
                    if (positions.Count < 2)
                        continue;

                    // Check whether all positions share one row.
                    int commonRow = positions[0][0];
                    bool sameRow = true;

                    foreach (int[] position in positions)
                    {
                        if (position[0] != commonRow)
                        {
                            sameRow = false;
                            break;
                        }
                    }

                    if (sameRow)
                    {
                        // Eliminate the candidate outside this block
                        // from the same row.
                        for (int column = 0; column < 9; column++)
                        {
                            if (column >= startColumn &&
                                column < startColumn + 3)
                                continue;

                            if (board.GetCell(commonRow, column) != 0)
                                continue;

                            if (candidateGrid.HasCandidate(
                                commonRow, column, number))
                            {
                                candidateGrid.RemoveCandidate(
                                    commonRow, column, number);

                                steps++;
                                RecordTechnique(
                                    SolvingTechnique.PointingPair);

                                return true;
                            }
                        }
                    }

                    // Check whether all positions share one column.
                    int commonColumn = positions[0][1];
                    bool sameColumn = true;

                    foreach (int[] position in positions)
                    {
                        if (position[1] != commonColumn)
                        {
                            sameColumn = false;
                            break;
                        }
                    }

                    if (sameColumn)
                    {
                        // Eliminate the candidate outside this block
                        // from the same column.
                        for (int row = 0; row < 9; row++)
                        {
                            if (row >= startRow &&
                                row < startRow + 3)
                                continue;

                            if (board.GetCell(row, commonColumn) != 0)
                                continue;

                            if (candidateGrid.HasCandidate(
                                row, commonColumn, number))
                            {
                                candidateGrid.RemoveCandidate(
                                    row, commonColumn, number);

                                steps++;
                                RecordTechnique(
                                    SolvingTechnique.PointingPair);

                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }


    private bool ApplyBoxLineReduction()
    {
        // -----------------------------------------
        // PART 1: ROW -> 3x3 BLOCK
        // -----------------------------------------

        for (int row = 0; row < 9; row++)
        {
            for (int number = 1; number <= 9; number++)
            {
                List<int[]> positions = new List<int[]>();

                // Find all possible positions in this row.
                for (int column = 0; column < 9; column++)
                {
                    if (board.GetCell(row, column) == 0 &&
                        candidateGrid.HasCandidate(row, column, number))
                    {
                        positions.Add(new int[] { row, column });
                    }
                }

                // Need at least two candidate positions.
                if (positions.Count < 2)
                    continue;

                // Check whether all positions belong to one block.
                int blockColumn = positions[0][1] / 3;
                bool sameBlock = true;

                foreach (int[] position in positions)
                {
                    if (position[1] / 3 != blockColumn)
                    {
                        sameBlock = false;
                        break;
                    }
                }

                if (!sameBlock)
                    continue;

                // Eliminate the candidate from other rows
                // inside the same 3x3 block.
                int startRow = (row / 3) * 3;
                int startColumn = blockColumn * 3;

                for (int r = startRow; r < startRow + 3; r++)
                {
                    if (r == row)
                        continue;

                    for (int c = startColumn;
                         c < startColumn + 3; c++)
                    {
                        if (board.GetCell(r, c) != 0)
                            continue;

                        if (candidateGrid.HasCandidate(r, c, number))
                        {
                            candidateGrid.RemoveCandidate(r, c, number);

                            steps++;
                            RecordTechnique(
                                SolvingTechnique.BoxLineReduction);

                            return true;
                        }
                    }
                }
            }
        }

        // -----------------------------------------
        // PART 2: COLUMN -> 3x3 BLOCK
        // -----------------------------------------

        for (int column = 0; column < 9; column++)
        {   
            for (int number = 1; number <= 9; number++)
            {
                List<int[]> positions = new List<int[]>();

                // Find all possible positions in this column.
                for (int row = 0; row < 9; row++)
                {
                    if (board.GetCell(row, column) == 0 &&
                        candidateGrid.HasCandidate(row, column, number))
                    {
                        positions.Add(new int[] { row, column });
                    }
                }

                if (positions.Count < 2)
                    continue;

                // Check whether all positions belong to one block.
                int blockRow = positions[0][0] / 3;
                bool sameBlock = true;

                foreach (int[] position in positions)
                {
                    if (position[0] / 3 != blockRow)
                    {
                        sameBlock = false;
                        break;
                    }
                }

                if (!sameBlock)
                    continue;

                // Eliminate the candidate from other columns
                // inside the same 3x3 block.
                int startRow = blockRow * 3;
                int startColumn = (column / 3) * 3;

                for (int r = startRow; r < startRow + 3; r++)
                {
                    for (int c = startColumn;
                         c < startColumn + 3; c++)
                    {
                        if (c == column)
                            continue;

                        if (board.GetCell(r, c) != 0)
                            continue;

                        if (candidateGrid.HasCandidate(r, c, number))
                        {
                            candidateGrid.RemoveCandidate(r, c, number);

                            steps++;
                            RecordTechnique(
                                SolvingTechnique.BoxLineReduction);

                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    private bool ApplyXWing()
    {
        // =========================================
        // PART 1: ROW-BASED X-WING
        // =========================================

        for (int firstRow = 0; firstRow < 8; firstRow++)
        {
            for (int number = 1; number <= 9; number++)
            {
                List<int> firstColumns = new List<int>();

                // Find candidate columns in the first row.
                for (int column = 0; column < 9; column++)
                {
                    if (board.GetCell(firstRow, column) == 0 &&
                        candidateGrid.HasCandidate(
                            firstRow, column, number))
                    {
                        firstColumns.Add(column);
                    }
                }

                // X-Wing requires exactly two positions.
                if (firstColumns.Count != 2)
                    continue;

                for (int secondRow = firstRow + 1;
                     secondRow < 9; secondRow++)
                {
                    List<int> secondColumns = new List<int>();

                    for (int column = 0; column < 9; column++)
                    {
                        if (board.GetCell(secondRow, column) == 0 &&
                            candidateGrid.HasCandidate(
                                secondRow, column, number))
                        {
                            secondColumns.Add(column);
                        }
                    }

                    if (secondColumns.Count != 2)
                        continue;

                    // Both rows must share the same two columns.
                    if (firstColumns[0] != secondColumns[0] ||
                        firstColumns[1] != secondColumns[1])
                        continue;

                    int columnA = firstColumns[0];
                    int columnB = firstColumns[1];

                    // Eliminate from column A and column B
                    // outside the two X-Wing rows.
                    for (int row = 0; row < 9; row++)
                    {
                        if (row == firstRow || row == secondRow)
                            continue;

                        if (board.GetCell(row, columnA) == 0 &&
                            candidateGrid.HasCandidate(
                                row, columnA, number))
                        {
                            candidateGrid.RemoveCandidate(
                                row, columnA, number);

                            steps++;
                            RecordTechnique(SolvingTechnique.XWing);
                            return true;
                        }

                        if (board.GetCell(row, columnB) == 0 &&
                            candidateGrid.HasCandidate(
                                row, columnB, number))
                        {
                            candidateGrid.RemoveCandidate(
                                row, columnB, number);

                            steps++;
                            RecordTechnique(SolvingTechnique.XWing);
                            return true;
                        }
                    }
                }
            }
        }

        // =========================================
        // PART 2: COLUMN-BASED X-WING
        // =========================================

        for (int firstColumn = 0; firstColumn < 8; firstColumn++)
        {
            for (int number = 1; number <= 9; number++)
            {
                List<int> firstRows = new List<int>();

                // Find candidate rows in the first column.
                for (int row = 0; row < 9; row++)
                {
                    if (board.GetCell(row, firstColumn) == 0 &&
                        candidateGrid.HasCandidate(
                            row, firstColumn, number))
                    {
                        firstRows.Add(row);
                    }
                }

                if (firstRows.Count != 2)
                    continue;

                for (int secondColumn = firstColumn + 1;
                     secondColumn < 9; secondColumn++)
                {
                    List<int> secondRows = new List<int>();

                    for (int row = 0; row < 9; row++)
                    {
                        if (board.GetCell(row, secondColumn) == 0 &&
                            candidateGrid.HasCandidate(
                                row, secondColumn, number))
                        {
                            secondRows.Add(row);
                        }
                    }

                    if (secondRows.Count != 2)
                        continue;

                    // Both columns must share the same two rows.
                    if (firstRows[0] != secondRows[0] ||
                        firstRows[1] != secondRows[1])
                        continue;

                    int rowA = firstRows[0];
                    int rowB = firstRows[1];

                    // Eliminate from row A and row B
                    // outside the two X-Wing columns.
                    for (int column = 0; column < 9; column++)
                    {
                        if (column == firstColumn ||
                            column == secondColumn)
                            continue;

                        if (board.GetCell(rowA, column) == 0 &&
                            candidateGrid.HasCandidate(
                                rowA, column, number))
                        {
                            candidateGrid.RemoveCandidate(
                                rowA, column, number);

                            steps++;
                            RecordTechnique(SolvingTechnique.XWing);
                            return true;
                        }

                        if (board.GetCell(rowB, column) == 0 &&
                            candidateGrid.HasCandidate(
                                rowB, column, number))
                        {
                            candidateGrid.RemoveCandidate(
                                rowB, column, number);

                            steps++;
                            RecordTechnique(SolvingTechnique.XWing);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }



    // Get the nine cells belonging to one row, column or block.
    private List<int[]> GetUnitCells(int unit)
    {
        List<int[]> cells = new List<int[]>();

        if (unit < 9)
        {
            // Rows: units 0 to 8.
            int row = unit;

            for (int column = 0; column < 9; column++)
                cells.Add(new int[] { row, column });
        }
        else if (unit < 18)
        {
            // Columns: units 9 to 17.
            int column = unit - 9;

            for (int row = 0; row < 9; row++)
                cells.Add(new int[] { row, column });
        }
        else
        {
            // Blocks: units 18 to 26.
            int block = unit - 18;
            int startRow = (block / 3) * 3;
            int startColumn = (block % 3) * 3;

            for (int row = startRow; row < startRow + 3; row++)
            {
                for (int column = startColumn;
                     column < startColumn + 3; column++)
                {
                    cells.Add(new int[] { row, column });
                }
            }
        }

        return cells;
    }

    private List<int[]> GetCandidatePositions(
        List<int[]> cells,
        int number)
    {
        List<int[]> positions = new List<int[]>();

        foreach (int[] cell in cells)
        {
            int row = cell[0];
            int column = cell[1];

            if (board.GetCell(row, column) == 0 &&
                candidateGrid.HasCandidate(row, column, number))
            {
                positions.Add(cell);
            }
        }

        return positions;
    }

    private bool SamePair(List<int> a, List<int> b)
    {
        return a.Count == 2 &&
               b.Count == 2 &&
               a[0] == b[0] &&
               a[1] == b[1];
    }

    private bool SamePositions(List<int[]> a, List<int[]> b)
    {
        return a.Count == 2 &&
               b.Count == 2 &&
               SameCell(a[0], b[0]) &&
               SameCell(a[1], b[1]);
    }

    private bool SameCell(int[] a, int[] b)
    {
        return a[0] == b[0] && a[1] == b[1];
    }

    private void PlaceNumber(
        int row,
        int column,
        int number,
        SolvingTechnique technique)
    {
        board.SetCell(row, column, number);

        // Remove all candidates from the filled cell.
        foreach (int candidate in
                 candidateGrid.GetCandidates(row, column))
        {
            candidateGrid.RemoveCandidate(
                row, column, candidate
            );
        }

        // Remove the placed number from peer cells.
        for (int i = 0; i < 27; i++)
        {
            List<int[]> cells = GetUnitCells(i);

            foreach (int[] cell in cells)
            {
                int peerRow = cell[0];
                int peerColumn = cell[1];

                if (peerRow == row && peerColumn == column)
                    continue;

                if (candidateGrid.HasCandidate(
                    peerRow, peerColumn, number))
                {
                    candidateGrid.RemoveCandidate(
                        peerRow, peerColumn, number
                    );
                }
            }
        }

        steps++;
        RecordTechnique(technique);
    }

    private void RecordTechnique(SolvingTechnique technique)
    {
        if (technique > highestTechnique)
            highestTechnique = technique;
    }

    private DifficultyAnalysisResult CreateResult(
        bool solved,
        string status)
    {
        return new DifficultyAnalysisResult(
            solved,
            highestTechnique,
            steps,
            CountEmptyCells(board),
            status
        );
    }

    private bool IsBoardConsistent(SudokuBoard currentBoard)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                int number = currentBoard.GetCell(row, column);

                if (number == 0)
                    continue;

                if (!currentBoard.IsValidPlacement(
                    row, column, number))
                    return false;
            }
        }

        return true;
    }

    private int CountEmptyCells(SudokuBoard currentBoard)
    {
        int count = 0;

        for (int i = 0; i < SudokuBoard.CellCount; i++)
        {
            if (currentBoard.GetCell(i) == 0)
                count++;
        }

        return count;
    }


    #if UNITY_EDITOR
    public bool DebugApplyNakedPair(
        SudokuBoard testBoard,
        CandidateGrid testCandidates)
    {
        board = testBoard;
        candidateGrid = testCandidates;
        steps = 0;
        highestTechnique = SolvingTechnique.None;

        return ApplyNakedPair();
    }

    public bool DebugApplyHiddenPair(
        SudokuBoard testBoard,
        CandidateGrid testCandidates)
    {
        board = testBoard;
        candidateGrid = testCandidates;
        steps = 0;
        highestTechnique = SolvingTechnique.None;

        return ApplyHiddenPair();
    }

    public bool DebugApplyPointingPair(
        SudokuBoard testBoard,
        CandidateGrid testCandidates)
    {
        board = testBoard;
        candidateGrid = testCandidates;
        steps = 0;
        highestTechnique = SolvingTechnique.None;

        return ApplyPointingPair();
    }

    public bool DebugApplyBoxLineReduction(
        SudokuBoard testBoard,
        CandidateGrid testCandidates)
    {
        board = testBoard;
        candidateGrid = testCandidates;
        steps = 0;
        highestTechnique = SolvingTechnique.None;

        return ApplyBoxLineReduction();
    }

    public bool DebugApplyXWing(
        SudokuBoard testBoard,
        CandidateGrid testCandidates)
    {
        board = testBoard;
        candidateGrid = testCandidates;
        steps = 0;
        highestTechnique = SolvingTechnique.None;

        return ApplyXWing();
    }
    
    #endif

}
