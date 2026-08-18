using System.Numerics;
using NewGamePlus.Entities;

namespace NewGamePlus.Tests.Entities;

public class InteractionCheckerTests
{
    [Fact]
    public void CanInteract_ReturnsFalse_WhenTooFarAway_EvenIfFacingCorrectly()
    {
        var playerPos = Vector2.Zero;
        var npcPos = new Vector2(1000f, 0f);

        Assert.False(InteractionChecker.CanInteract(playerPos, Direction.Right, npcPos));
    }

    [Fact]
    public void CanInteract_ReturnsFalse_WhenCloseButFacingAway()
    {
        var playerPos = Vector2.Zero;
        var npcPos = new Vector2(40f, 0f); // NPC is to the right

        Assert.False(InteractionChecker.CanInteract(playerPos, Direction.Left, npcPos));
    }

    [Fact]
    public void CanInteract_ReturnsTrue_WhenCloseAndFacingCorrectly()
    {
        var playerPos = Vector2.Zero;
        var npcPos = new Vector2(40f, 0f);

        Assert.True(InteractionChecker.CanInteract(playerPos, Direction.Right, npcPos));
    }

    [Theory]
    [InlineData(0f, -40f, Direction.Up)]
    [InlineData(0f, 40f, Direction.Down)]
    [InlineData(-40f, 0f, Direction.Left)]
    [InlineData(40f, 0f, Direction.Right)]
    public void CanInteract_ReturnsTrue_ForAllFourCardinalDirections(float dx, float dy, Direction facing)
    {
        var playerPos = Vector2.Zero;
        var npcPos = new Vector2(dx, dy);

        Assert.True(InteractionChecker.CanInteract(playerPos, facing, npcPos));
    }

    [Fact]
    public void CanInteract_ReturnsFalse_JustOutsideInteractionRange()
    {
        var playerPos = Vector2.Zero;
        var npcPos = new Vector2(InteractionChecker.InteractionRange + 1f, 0f);

        Assert.False(InteractionChecker.CanInteract(playerPos, Direction.Right, npcPos));
    }

    [Fact]
    public void CanInteract_ReturnsTrue_JustInsideInteractionRange()
    {
        var playerPos = Vector2.Zero;
        var npcPos = new Vector2(InteractionChecker.InteractionRange - 1f, 0f);

        Assert.True(InteractionChecker.CanInteract(playerPos, Direction.Right, npcPos));
    }
}
