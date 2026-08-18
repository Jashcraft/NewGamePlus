namespace NewGamePlus.Core;

public interface IGameState
{
    void OnEnter();
    void OnExit();
    void Update(float dt, InputSnapshot input);
    void Draw();
}
