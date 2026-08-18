using NewGamePlus.Core;

namespace NewGamePlus.Tests;

public class HarnessSanityTests
{
    [Fact]
    public void TestHarnessRuns()
    {
        Assert.True(true);
    }

    [Fact]
    public void CanReferenceMainProjectCode()
    {
        Assert.Equal(800, GameLoop.ScreenWidth);
        Assert.Equal(600, GameLoop.ScreenHeight);
    }
}
