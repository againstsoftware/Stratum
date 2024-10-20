using Unity.Netcode;

internal struct NetworkPlayerAction : INetworkSerializable
{
    public PlayerCharacter Actor;
    public int ActionItemID;
    public Receiver[] Receivers;
    public int EffectsIndex;

    public NetworkPlayerAction(PlayerAction action, GameConfig config)
    {
        Actor = action.Actor;
        Receivers = action.Receivers;
        EffectsIndex = action.EffectsIndex;
        ActionItemID = config.ActionItemToID(action.ActionItem);
    }

    public PlayerAction ToPlayerAction(GameConfig config)
    {
        return new PlayerAction(Actor, config.IDToActionItem(ActionItemID), Receivers, EffectsIndex);
    }
        
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Actor);
        serializer.SerializeValue(ref ActionItemID);
        serializer.SerializeValue(ref Receivers);
        serializer.SerializeValue(ref EffectsIndex);
    }
}