using System;
using System.Collections.Generic;
public interface IRulesSystem : IService
{
    public event Action<PlayerCharacter[]> OnGameOver;
    
    // comprobación local
    public bool IsValidAction(PlayerAction action); //esta comprueba en local

    
    
    //esta comprueba en host (rpc) y la ejecuta si es valida
    //si no aprueba la jugada, hay discrepancia entre cliente y host, se expulsa al jugador y se cancela la partida por chetos
    public void PerformAction(PlayerAction action);
    public void SetForcedAction(IReadOnlyList<PlayerAction> forcedActions);
    public void DisableForcedAction();

    public void RegisterRoundEndObserver(IRoundEndObserverEffectCommand reo);
    public void RemoveRoundEndObserver(IRoundEndObserverEffectCommand reo);
}