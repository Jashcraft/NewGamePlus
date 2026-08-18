using NewGamePlus.States;

namespace NewGamePlus.Tests.States;

public class DialogueProgressTests
{
    [Fact]
    public void CurrentLine_IsFirstLine_Initially()
    {
        var progress = new DialogueProgress(new[] { "a", "b", "c" });

        Assert.Equal("a", progress.CurrentLine);
    }

    [Fact]
    public void Advance_MovesToNextLine_AndReturnsTrue_WhenMoreLinesRemain()
    {
        var progress = new DialogueProgress(new[] { "a", "b", "c" });

        bool hasMore = progress.Advance();

        Assert.True(hasMore);
        Assert.Equal("b", progress.CurrentLine);
    }

    [Fact]
    public void Advance_ReturnsFalse_WhenAdvancingPastTheLastLine()
    {
        var progress = new DialogueProgress(new[] { "only line" });

        bool hasMore = progress.Advance();

        Assert.False(hasMore);
    }

    [Fact]
    public void Advance_ThroughAllLines_ReturnsFalseOnlyOnTheFinalAdvance()
    {
        var progress = new DialogueProgress(new[] { "a", "b" });

        Assert.True(progress.Advance());
        Assert.Equal("b", progress.CurrentLine);

        Assert.False(progress.Advance());
    }
}
