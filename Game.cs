namespace LifeSharp;

public class Game
{
    public bool[,] Board;

    public Game(int size)
    {
        Size = size;
        Board = new bool[Size, Size];
    }

    public int Size { get; }

    public void ClearBoard()
    {
        Board = new bool[Size, Size];
    }

    public void BoardStep()
    {
        var boardCopy = (bool[,])Board.Clone();

        for (var y = 0; y < Board.GetLength(0); y++)
        for (var x = 0; x < Board.GetLength(1); x++)
        {
            var neighbors = GetAliveNeighbors(x, y);
            var cellValue = Board[y, x];

            if (cellValue)
            {
                if (!(neighbors is 2 or 3)) cellValue = false;
            }
            else
            {
                if (neighbors == 3) cellValue = true;
            }

            boardCopy[y, x] = cellValue;
        }

        Board = boardCopy;
    }

    public int GetAliveNeighbors(int x, int y)
    {
        return new[]
        {
            SafeBoardAccess(x - 1, y - 1), SafeBoardAccess(x, y - 1), SafeBoardAccess(x + 1, y - 1),
            SafeBoardAccess(x - 1, y), SafeBoardAccess(x + 1, y),
            SafeBoardAccess(x - 1, y + 1), SafeBoardAccess(x, y + 1), SafeBoardAccess(x + 1, y + 1)
        }.Count(v => v);
    }

    public bool SwitchValue(int x, int y)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size)
            throw new ArgumentException("Value position out of range for the board size");

        var originalValue = Board[y, x];

        Board[y, x] = !originalValue;

        return !originalValue;
    }

    private bool SafeBoardAccess(int x, int y)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size) return false;

        return Board[y, x];
    }
}