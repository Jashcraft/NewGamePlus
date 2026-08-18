using Raylib_cs;

namespace NewGamePlus.Core;

// Latches edge-triggered key presses so they survive until an Update()
// call actually consumes them. Raylib.IsKeyPressed only reports true on
// the single real frame a key transitions down, but fixed-timestep
// Update() calls don't happen on every real frame once rendering runs
// faster than 60Hz - a press can land on a real frame with zero Update()
// calls. CaptureFrame merges (not replaces) into the latch set each real
// frame in GameLoop, so a press keeps carrying forward across frames
// until some Update() call finally consumes it via Consume().
public class InputSnapshot
{
    private readonly HashSet<KeyboardKey> _pressedKeys;

    public InputSnapshot() : this(new HashSet<KeyboardKey>())
    {
    }

    public InputSnapshot(HashSet<KeyboardKey> pressedKeys)
    {
        _pressedKeys = pressedKeys;
    }

    public void CaptureFrame(params KeyboardKey[] keysToTrack)
    {
        foreach (var key in keysToTrack)
        {
            if (Raylib.IsKeyPressed(key))
            {
                _pressedKeys.Add(key);
            }
        }
    }

    public bool WasPressed(KeyboardKey key) => _pressedKeys.Contains(key);

    public void Consume() => _pressedKeys.Clear();
}
