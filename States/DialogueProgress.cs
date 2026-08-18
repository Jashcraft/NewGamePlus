namespace NewGamePlus.States;

public class DialogueProgress
{
    private readonly IReadOnlyList<string> _lines;
    private int _currentIndex;

    public DialogueProgress(IReadOnlyList<string> lines)
    {
        _lines = lines;
        _currentIndex = 0;
    }

    public string CurrentLine => _lines[_currentIndex];

    // Returns false once advanced past the last line - the caller should
    // close the dialogue rather than try to show a line that no longer exists.
    public bool Advance()
    {
        _currentIndex++;
        return _currentIndex < _lines.Count;
    }
}
