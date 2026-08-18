using System.Numerics;

namespace NewGamePlus.World;

public static class TileCollision
{
    // Water blocks movement, same as walls - no swim ability exists yet.
    private static bool IsBlockingTile(int tileId)
    {
        return tileId == TileIds.Wall || tileId == TileIds.Water || tileId == TileIds.Void;
    }

    public static bool IsBlocked(Tilemap map, Vector2 center, Vector2 size)
    {
        float left = center.X - size.X / 2f;
        float right = center.X + size.X / 2f;
        float top = center.Y - size.Y / 2f;
        float bottom = center.Y + size.Y / 2f;

        int minTileX = (int)MathF.Floor(left / Tilemap.TileSize);
        int maxTileX = (int)MathF.Floor((right - 0.001f) / Tilemap.TileSize);
        int minTileY = (int)MathF.Floor(top / Tilemap.TileSize);
        int maxTileY = (int)MathF.Floor((bottom - 0.001f) / Tilemap.TileSize);

        for (int tx = minTileX; tx <= maxTileX; tx++)
        {
            for (int ty = minTileY; ty <= maxTileY; ty++)
            {
                if (IsBlockingTile(map.GetTile(tx, ty)))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
