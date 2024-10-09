
public interface ICommunicationSystem : IService
{
    public bool IsAuthority { get; }
    //manda la accion para ser comprobada y en caso correcto se ejecuta
    public void SendActionToAuthority(PlayerAction action);

    public void SendTurnChange(PlayerCharacter playerOnTurn);

}
