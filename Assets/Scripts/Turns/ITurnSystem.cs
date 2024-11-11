using System;
public interface ITurnSystem : IService
{
    public PlayerCharacter PlayerOnTurn { get; }
    public event Action<PlayerCharacter> OnTurnChanged, OnActionEnded;
    public event Action OnGameStart;

    public void StartGame();

    public void EndAction();

    public void ChangeTurn(PlayerCharacter playerOnTurn);
}
