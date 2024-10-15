
public interface ICommunicationSystem : IService
{
    public bool IsAuthority { get; }
    public bool IsRNGSynced { get; }
    
    public void SyncRNGs();
    
    //manda la accion para ser comprobada y en caso correcto se ejecuta
    public void SendActionToAuthority(PlayerAction action);

    public void SendTurnChange(PlayerCharacter playerOnTurn);

}
