using System;


public static class GameManager
{

    public static bool IsValidAction(ITurnAction action, IActionReceiver receiver) //esta comprueba en local
    {
        return true;
    }
    
    
    //esta comprueba en host (rpc) y la ejecuta si es valida
    //si no aprueba la jugada, hay discrepancia entre cliente y host, se expulsa al jugador y se cancela la partida por chetos
    public static void TryPerformAction(ITurnAction action, IActionReceiver receiver, Action onApproved)
    {
        onApproved();
    }
}