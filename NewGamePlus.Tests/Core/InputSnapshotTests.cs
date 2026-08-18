using NewGamePlus.Core;
using Raylib_cs;

namespace NewGamePlus.Tests.Core;

public class InputSnapshotTests
{
    [Fact]
    public void WasPressed_ReturnsTrue_ForKeyInSnapshot()
    {
        var input = new InputSnapshot(new HashSet<KeyboardKey> { KeyboardKey.E });

        Assert.True(input.WasPressed(KeyboardKey.E));
    }

    [Fact]
    public void WasPressed_ReturnsFalse_ForKeyNotInSnapshot()
    {
        var input = new InputSnapshot(new HashSet<KeyboardKey> { KeyboardKey.E });

        Assert.False(input.WasPressed(KeyboardKey.Enter));
    }

    [Fact]
    public void Consume_ClearsAllLatchedKeys()
    {
        var input = new InputSnapshot(new HashSet<KeyboardKey> { KeyboardKey.E, KeyboardKey.Enter });

        input.Consume();

        Assert.False(input.WasPressed(KeyboardKey.E));
        Assert.False(input.WasPressed(KeyboardKey.Enter));
    }
}
