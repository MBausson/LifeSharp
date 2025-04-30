using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace LifeSharp;

public class GameWindow
{
    private readonly Vector2u _size;
    private readonly RenderWindow _window;
    private readonly Game _game;
    private readonly int _cellSize = 10;
    private readonly Font _font = new("resources/font.ttf");
    private Clock _clock = new();
    private float _stepSpeed = 1;
    private bool _drawNeighbors;
    private bool _drawGrid;
    private bool _keepIterating;
    private Color _clearColor = Color.Black;
    private Color _gridColor = new Color(255, 255, 255, 100);
    private RectangleShape CellShape = new(new Vector2f(0, 0))
    {
        FillColor = Color.White,
        OutlineColor = Color.Black,
        OutlineThickness = 1,
    };

    public GameWindow(Vector2u size)
    {
        _size = size;
        CellShape.Size = new Vector2f(_cellSize, _cellSize);
        _window = new RenderWindow(new VideoMode(_size.X, _size.Y), "LifeSharp !", Styles.Titlebar | Styles.Close);

        _window.Closed += (_, _) => _window.Close();
        _window.KeyPressed += WindowOnKeyPressed;
        _window.MouseButtonPressed += WindowOnMouseButtonPressed;

        _game = new Game((int)_size.X / _cellSize);
    }

    public void Run()
    {
        Time elapsedTime = Time.Zero;

        while (_window.IsOpen)
        {
            var deltaTime = _clock.Restart();
            elapsedTime += deltaTime;

            _window.DispatchEvents();
            _window.Clear(_clearColor);

            DrawBoard();
            if (_drawNeighbors) DrawBoardNeighbors();
            if (_drawGrid) DrawGrid();

            if (_keepIterating && elapsedTime.AsSeconds() >= 0.3f / _stepSpeed)
            {
                _game.BoardStep();
                elapsedTime = Time.Zero;
            }

            _window.Display();
        }
    }

    private void DrawBoard()
    {
        for (int y = 0; y < _game.Board.GetLength(0); y++)
        {
            for (int x = 0; x < _game.Board.GetLength(1); x++)
            {
                var value = _game.Board[y, x];

                if (value)
                    _window.Draw(GetCellShape(new Vector2f(x * 10, y * 10)));
            }
        }
    }

    private void DrawBoardNeighbors()
    {
        for (int y = 0; y < _game.Board.GetLength(0); y++)
        {
            for (int x = 0; x < _game.Board.GetLength(1); x++)
            {
                var n = _game.GetAliveNeighbors(x, y);

                _window.Draw(TextShape(new Vector2f(x * 10, y * 10), n.ToString()));
            }
        }
    }

    private void DrawGrid()
    {
        //  Horizontal lines
        var line = new RectangleShape
        {
            FillColor = _gridColor,
            Size = new Vector2f(_cellSize * _size.X, 1)
        };

        for (int y = 0; y < _size.Y; y++)
        {
            line.Position = new Vector2f(0, y * _cellSize);
            _window.Draw(line);
        }

        //  Vertical lines
        line.Size = new Vector2f(1, _cellSize * _size.Y);

        for (int x = 0; x < _size.X; x++)
        {
            line.Position = new Vector2f(x * _cellSize, 0);
            _window.Draw(line);
        }
    }

    private void WindowOnMouseButtonPressed(object? _, MouseButtonEventArgs e)
    {
        var cellPosition = new Vector2i(e.X / 10, e.Y / 10);
        _game.SwitchValue(cellPosition.X, cellPosition.Y);
    }

    private void WindowOnKeyPressed(object? _, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Escape:
                _window.Close();
                break;

            case Keyboard.Key.N:
                _drawNeighbors = !_drawNeighbors;
                break;

            case Keyboard.Key.G:
                _drawGrid = !_drawGrid;
                break;

            case Keyboard.Key.Space:
                _keepIterating = !_keepIterating;
                break;

            case Keyboard.Key.Up:
                _stepSpeed *= 1.25f;
                break;

            case Keyboard.Key.Down:
                _stepSpeed *= 0.75f;
                break;

            case Keyboard.Key.C:
                _game.ClearBoard();
                break;

            case Keyboard.Key.LShift:
                if (_clearColor == Color.Black)
                {
                    _clearColor = Color.White;
                    CellShape.FillColor = Color.Black;
                    _gridColor = new Color(0, 0, 0, 100);
                }
                else
                {
                    _clearColor = Color.Black;
                    CellShape.FillColor = Color.White;
                    _gridColor = new Color(255, 255, 255, 100);
                }
                break;
        }
    }

    private Text TextShape(Vector2f position, string text) => new (text, _font)
    {
        Position = position,
        FillColor = Color.Green,
        CharacterSize = 11
    };

    private RectangleShape GetCellShape(Vector2f position)
    {
        CellShape.Position = position;
        return CellShape;
    }
}
