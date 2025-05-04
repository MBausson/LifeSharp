using SFML.Graphics;
using SFML.System;

namespace LifeSharp;

public class Graphics(Vector2f cellSize)
{
    private static readonly Font Font = new("resources/font.ttf");

    private bool _blackBackground = true;
    public Color ClearColor { get; private set; } = Color.Black;
    public Color GridColor { get; private set; } = new(255, 255, 255, 100);

    private RectangleShape CellShape { get; } = new()
    {
        FillColor = Color.White,
        OutlineColor = Color.Black,
        OutlineThickness = 1,
        Size = cellSize
    };

    private Text NeighborsTextShape { get; } = new()
    {
        FillColor = Color.Green,
        CharacterSize = 11
    };

    private Text GameStateTextShape { get; } = new()
    {
        FillColor = Color.Red,
        CharacterSize = 24,
        Font = Font
    };

    public void ToggleColorMode()
    {
        _blackBackground = !_blackBackground;

        if (_blackBackground)
        {
            ClearColor = Color.White;
            CellShape.FillColor = Color.Black;
            GridColor = new Color(0, 0, 0, 100);
            return;
        }

        ClearColor = Color.Black;
        CellShape.FillColor = Color.White;
        GridColor = new Color(255, 255, 255, 100);
    }

    public RectangleShape GetCellShape(Vector2f position)
    {
        CellShape.Position = position;
        return CellShape;
    }

    public Text GetNeighborsTextShape(Vector2f position, string text)
    {
        NeighborsTextShape.Position = position;
        NeighborsTextShape.DisplayedString = text;

        return NeighborsTextShape;
    }

    public Text GetGameStateTextShape(string content, Vector2f position)
    {
        GameStateTextShape.Position = position;
        GameStateTextShape.DisplayedString = content;

        return GameStateTextShape;
    }
}
