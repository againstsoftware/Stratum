
using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour, ITurnSystem, ICommunicationSystem
{
    public PlayerCharacter PlayerOnTurn { get; }
    public bool IsAuthority => true;
    public bool IsRNGSynced => true;
    
    public event Action<PlayerCharacter> OnTurnChanged;
    public event Action<PlayerCharacter> OnActionEnded;
    
    public event Action OnGameStart;
    
    public void StartGame()
    {
        
        
        
        OnGameStart?.Invoke();
    }
    
    public void SendActionToAuthority(PlayerAction action)
    {
        ServiceLocator.Get<IExecutor>().ExecutePlayerActionEffects(action);
    }

    public void EndAction()
    {
        
    }

    
    
    
    public void ChangeTurn(PlayerCharacter playerOnTurn) {}
    public void SyncRNGs() {}
    public void SendTurnChange(PlayerCharacter playerOnTurn) {}
}
