namespace NewGamePlus.World;

public class Tilemap
{
    public const int TileSize = 48;

    private readonly int[,] _tiles;

    public int Width { get; }
    public int Height { get; }

    public Tilemap(int[,] tiles)
    {
        _tiles = tiles;
        Width = tiles.GetLength(0);
        Height = tiles.GetLength(1);
    }

    public int GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return TileIds.Void;
        }

        return _tiles[x, y];
    }

    public static Tilemap CreateTestMap()
    {
        const int width = 20;
        const int height = 15;
        var tiles = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isBorder = x == 0 || y == 0 || x == width - 1 || y == height - 1;
                tiles[x, y] = isBorder ? TileIds.Wall : TileIds.Grass;
            }
        }

        // A water patch, just for visual variety.
        for (int x = 8; x <= 12; x++)
        {
            for (int y = 6; y <= 8; y++)
            {
                tiles[x, y] = TileIds.Water;
            }
        }

        return new Tilemap(tiles);
    }
}
