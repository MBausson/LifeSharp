using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace LifeSharp;

public class GameWindow
{
    private const int CellSize = 10;
    private readonly Clock _clock = new();
    private readonly Game _game;
    private readonly GameState _gameState = new();
    private readonly Graphics _graphics;
    private readonly RenderWindow _window;
    private readonly Vector2u _windowSize;

    public GameWindow(Vector2u windowSize)
    {
        _windowSize = windowSize;
        _window = new RenderWindow(new VideoMode(_windowSize.X, _windowSize.Y), "LifeSharp !",
            Styles.Titlebar | Styles.Close);

        _graphics = new Graphics(new Vector2f(CellSize, CellSize));

        _window.Closed += (_, _) => _window.Close();
        _window.KeyPressed += WindowOnKeyPressed;
        _window.MouseButtonPressed += WindowOnMouseButtonPressed;

        _game = new Game((int)_windowSize.X / CellSize);
    }

    public void Run()
    {
        var elapsedTime = Time.Zero;

        while (_window.IsOpen)
        {
            var deltaTime = _clock.Restart();
            elapsedTime += deltaTime;

            _window.DispatchEvents();
            _window.Clear(_graphics.ClearColor);

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
                _window.Draw(_graphics.GetCellShape(new Vector2f(x * 10, y * 10)));
        }
    }

    private void DrawBoardNeighbors()
    {
        for (var y = 0; y < _game.Board.GetLength(0); y++)
        for (var x = 0; x < _game.Board.GetLength(1); x++)
        {
            var n = _game.GetAliveNeighbors(x, y);

            _window.Draw(_graphics.GetNeighborsTextShape(new Vector2f(x * 10, y * 10), n.ToString()));
        }
    }

    private void DrawGrid()
    {
        //  Horizontal lines
        var line = new RectangleShape
        {
            FillColor = _graphics.GridColor,
            Size = new Vector2f(CellSize * _windowSize.X, 1)
        };

        for (var y = 0; y < _windowSize.Y; y++)
        {
            line.Position = new Vector2f(0, y * CellSize);
            _window.Draw(line);
        }

        //  Vertical lines
        line.Size = new Vector2f(1, CellSize * _windowSize.Y);

        for (var x = 0; x < _windowSize.X; x++)
        {
            line.Position = new Vector2f(x * CellSize, 0);
            _window.Draw(line);
        }
    }

    private void DrawGameState()
    {
        _window.Draw(_graphics.GetGameStateTextShape($"Speed : {_gameState.StepSpeed:F1}", new Vector2f(10, 10)));
        _window.Draw(_graphics.GetGameStateTextShape($"Iterations : {_gameState.Iterations}", new Vector2f(10, 34)));
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
                _graphics.ToggleColorMode();
                break;
        }
    }
}
