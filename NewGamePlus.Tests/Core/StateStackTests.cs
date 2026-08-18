using NewGamePlus.Core;

namespace NewGamePlus.Tests.Core;

public class StateStackTests
{
    private class FakeGameState : IGameState
    {
        private readonly List<string>? _callLog;
        private readonly string _name;

        public int EnterCount;
        public int ExitCount;
        public int UpdateCount;
        public int DrawCount;

        public FakeGameState(string name = "state", List<string>? callLog = null)
        {
            _name = name;
            _callLog = callLog;
        }

        public void OnEnter() => EnterCount++;
        public void OnExit() => ExitCount++;

        public void Update(float dt)
        {
            UpdateCount++;
            _callLog?.Add($"{_name}.Update");
        }

        public void Draw()
        {
            DrawCount++;
            _callLog?.Add($"{_name}.Draw");
        }
    }

    [Fact]
    public void Push_CallsOnEnter_AndBecomesCurrent()
    {
        var stack = new StateStack();
        var state = new FakeGameState();

        stack.Push(state);

        Assert.Equal(1, state.EnterCount);
        Assert.Same(state, stack.Current);
    }

    [Fact]
    public void Pop_CallsOnExit_AndRestoresPreviousAsCurrent()
    {
        var stack = new StateStack();
        var bottom = new FakeGameState("bottom");
        var top = new FakeGameState("top");
        stack.Push(bottom);
        stack.Push(top);

        stack.Pop();

        Assert.Equal(1, top.ExitCount);
        Assert.Same(bottom, stack.Current);
    }

    [Fact]
    public void Pop_OnEmptyStack_DoesNotThrow()
    {
        var stack = new StateStack();

        var exception = Record.Exception(() => stack.Pop());

        Assert.Null(exception);
        Assert.Null(stack.Current);
    }

    [Fact]
    public void Update_OnlyRoutesToTopState()
    {
        var stack = new StateStack();
        var bottom = new FakeGameState("bottom");
        var top = new FakeGameState("top");
        stack.Push(bottom);
        stack.Push(top);

        stack.Update(0.016f);

        Assert.Equal(0, bottom.UpdateCount);
        Assert.Equal(1, top.UpdateCount);
    }

    [Fact]
    public void Draw_RoutesToAllStates_BottomToTop()
    {
        var stack = new StateStack();
        var callLog = new List<string>();
        var bottom = new FakeGameState("bottom", callLog);
        var top = new FakeGameState("top", callLog);
        stack.Push(bottom);
        stack.Push(top);

        stack.Draw();

        Assert.Equal(1, bottom.DrawCount);
        Assert.Equal(1, top.DrawCount);
        Assert.Equal(new[] { "bottom.Draw", "top.Draw" }, callLog);
    }
}
