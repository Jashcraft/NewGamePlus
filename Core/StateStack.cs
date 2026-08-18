namespace NewGamePlus.Core;

// Only the top state Updates (a paused state below it should freeze, not
// keep simulating) - but every state Draws, bottom to top, so a paused
// Overworld still renders as a backdrop behind e.g. a Dialogue box.
public class StateStack
{
    private readonly List<IGameState> _states = new();

    public IGameState? Current => _states.Count > 0 ? _states[^1] : null;

    public void Push(IGameState state)
    {
        _states.Add(state);
        state.OnEnter();
    }

    public void Pop()
    {
        if (_states.Count == 0)
        {
            return;
        }

        var top = _states[^1];
        _states.RemoveAt(_states.Count - 1);
        top.OnExit();
    }

    public void Update(float dt, InputSnapshot input)
    {
        Current?.Update(dt, input);

        // A single real-frame key press can drive multiple Update() calls
        // (accumulator catch-up), so the snapshot is only actionable once.
        input.Consume();
    }

    public void Draw()
    {
        foreach (var state in _states)
        {
            state.Draw();
        }
    }
}
