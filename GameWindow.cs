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
    private readonly GameState _gameState = new();
    private readonly Clock _clock = new();
    private Color _clearColor = Color.Black;
    private Color _gridColor = new(255, 255, 255, 100);

    private readonly RectangleShape _cellShape = new(new Vector2f(0, 0))
    {
        FillColor = Color.White,
        OutlineColor = Color.Black,
        OutlineThickness = 1
    };

    public GameWindow(Vector2u size)
    {
        _size = size;
        _cellShape.Size = new Vector2f(_cellSize, _cellSize);
        _window = new RenderWindow(new VideoMode(_size.X, _size.Y), "LifeSharp !", Styles.Titlebar | Styles.Close);

        _window.Closed += (_, _) => _window.Close();
        _window.KeyPressed += WindowOnKeyPressed;
        _window.MouseButtonPressed += WindowOnMouseButtonPressed;

        _game = new Game((int)_size.X / _cellSize);
    }

    public void Run()
    {
        var elapsedTime = Time.Zero;

        while (_window.IsOpen)
        {
            var deltaTime = _clock.Restart();
            elapsedTime += deltaTime;

            _window.DispatchEvents();
            _window.Clear(_clearColor);

            DrawBoard();
            if (_gameState.DrawNeighbors) DrawBoardNeighbors();
            if (_gameState.DrawGrid) DrawGrid();
            if (_gameState.DrawGameState) DrawGameState();

            if (_gameState.KeepIterating && elapsedTime.AsSeconds() >= 0.3f / _gameState.StepSpeed)
            {
                _game.BoardStep();
                _gameState.Iterations += 1;
                elapsedTime = Time.Zero;
            }

            _window.Display();
        }
    }

    private void DrawBoard()
    {
        for (var y = 0; y < _game.Board.GetLength(0); y++)
        for (var x = 0; x < _game.Board.GetLength(1); x++)
        {
            var value = _game.Board[y, x];

            if (value)
                _window.Draw(GetCellShape(new Vector2f(x * 10, y * 10)));
        }
    }

    private void DrawBoardNeighbors()
    {
        for (var y = 0; y < _game.Board.GetLength(0); y++)
        for (var x = 0; x < _game.Board.GetLength(1); x++)
        {
            var n = _game.GetAliveNeighbors(x, y);

            _window.Draw(TextShape(new Vector2f(x * 10, y * 10), n.ToString()));
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

        for (var y = 0; y < _size.Y; y++)
        {
            line.Position = new Vector2f(0, y * _cellSize);
            _window.Draw(line);
        }

        //  Vertical lines
        line.Size = new Vector2f(1, _cellSize * _size.Y);

        for (var x = 0; x < _size.X; x++)
        {
            line.Position = new Vector2f(x * _cellSize, 0);
            _window.Draw(line);
        }
    }

    private void DrawGameState()
    {
        var stepSpeedText = new Text($"Speed : {_gameState.StepSpeed:F1}", _font)
        {
            FillColor = Color.Red,
            Position = new Vector2f(10, 10),
            CharacterSize = 24
        };

        var iterationsText = new Text($"Iterations : {_gameState.Iterations}", _font)
        {
            FillColor = Color.Red,
            Position = new Vector2f(10, 34),
            CharacterSize = 24
        };

        _window.Draw(stepSpeedText);
        _window.Draw(iterationsText);
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
            //  Close the game with Escape
            case Keyboard.Key.Escape:
                _window.Close();
                break;

            //  Toggle to show the neighbors of each cell
            case Keyboard.Key.N:
                _gameState.DrawNeighbors = !_gameState.DrawNeighbors;
                break;

            //  Toggle to show the grid
            case Keyboard.Key.G:
                _gameState.DrawGrid = !_gameState.DrawGrid;
                break;

            //  Toggle to show game state's information
            case Keyboard.Key.H:
                _gameState.DrawGameState = !_gameState.DrawGameState;
                break;

            //  Toggle to enable game iterations
            case Keyboard.Key.Space:
                _gameState.KeepIterating = !_gameState.KeepIterating;
                break;

            //  Speed-up game's iterations
            case Keyboard.Key.Up:
                _gameState.StepSpeed *= 1.25f;
                break;

            //  Slow-down game's iterations
            case Keyboard.Key.Down:
                _gameState.StepSpeed *= 0.75f;
                break;

            //  Clear the game's board
            case Keyboard.Key.C:
                _game.ClearBoard();
                break;

            //  Toggle to display the game board in black or white
            case Keyboard.Key.LShift:
                ToggleBlackWhite();
                break;
        }
    }

    private void ToggleBlackWhite()
    {
        if (_clearColor == Color.Black)
        {
            _clearColor = Color.White;
            _cellShape.FillColor = Color.Black;
            _gridColor = new Color(0, 0, 0, 100);
            return;
        }

        _clearColor = Color.Black;
        _cellShape.FillColor = Color.White;
        _gridColor = new Color(255, 255, 255, 100);
    }

    private Text TextShape(Vector2f position, string text)
    {
        return new Text(text, _font)
        {
            Position = position,
            FillColor = Color.Green,
            CharacterSize = 11
        };
    }

    private RectangleShape GetCellShape(Vector2f position)
    {
        _cellShape.Position = position;
        return _cellShape;
    }
}
