using Raylib_cs;

namespace NewGamePlus.World;

public static class TilemapRenderer
{
    public static void Draw(Tilemap map)
    {
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                Color color = map.GetTile(x, y) switch
                {
                    TileIds.Grass => Color.Green,
                    TileIds.Wall => Color.DarkGray,
                    TileIds.Water => Color.Blue,
                    _ => Color.Magenta,
                };

                Raylib.DrawRectangle(x * Tilemap.TileSize, y * Tilemap.TileSize, Tilemap.TileSize, Tilemap.TileSize, color);
            }
        }
    }
}
