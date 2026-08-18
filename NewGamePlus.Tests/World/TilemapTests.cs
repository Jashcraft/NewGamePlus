using NewGamePlus.World;

namespace NewGamePlus.Tests.World;

public class TilemapTests
{
    [Fact]
    public void GetTile_ReturnsCorrectId_ForKnownCoordinates()
    {
        var tiles = new int[,]
        {
            { TileIds.Grass, TileIds.Wall },
            { TileIds.Water, TileIds.Grass },
        };
        var map = new Tilemap(tiles);

        Assert.Equal(TileIds.Grass, map.GetTile(0, 0));
        Assert.Equal(TileIds.Wall, map.GetTile(0, 1));
        Assert.Equal(TileIds.Water, map.GetTile(1, 0));
        Assert.Equal(TileIds.Grass, map.GetTile(1, 1));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(2, 0)]
    [InlineData(0, 2)]
    public void GetTile_OutOfBounds_ReturnsVoidTileId(int x, int y)
    {
        var tiles = new int[,] { { 0, 0 }, { 0, 0 } };
        var map = new Tilemap(tiles);

        Assert.Equal(TileIds.Void, map.GetTile(x, y));
    }

    [Fact]
    public void WidthAndHeight_MatchInputGrid()
    {
        var tiles = new int[3, 5];
        var map = new Tilemap(tiles);

        Assert.Equal(3, map.Width);
        Assert.Equal(5, map.Height);
    }

    [Fact]
    public void CreateTestMap_HasExpectedDimensions()
    {
        var map = Tilemap.CreateTestMap();

        Assert.Equal(20, map.Width);
        Assert.Equal(15, map.Height);
    }

    [Fact]
    public void CreateTestMap_BorderIsWalled()
    {
        var map = Tilemap.CreateTestMap();

        for (int x = 0; x < map.Width; x++)
        {
            Assert.Equal(TileIds.Wall, map.GetTile(x, 0));
            Assert.Equal(TileIds.Wall, map.GetTile(x, map.Height - 1));
        }

        for (int y = 0; y < map.Height; y++)
        {
            Assert.Equal(TileIds.Wall, map.GetTile(0, y));
            Assert.Equal(TileIds.Wall, map.GetTile(map.Width - 1, y));
        }
    }
}
