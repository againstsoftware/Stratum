using Unity.Netcode;

public struct Receiver : INetworkSerializable
{
    public ValidDropLocation Location;
    public PlayerCharacter LocationOwner;
    public int Index, SecondIndex;

    public Receiver(ValidDropLocation validDropLocation, PlayerCharacter locationOwner, int index, int secondIndex)
    {
        Location = validDropLocation;
        LocationOwner = locationOwner;
        Index = index;
        SecondIndex = secondIndex;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Location);
        serializer.SerializeValue(ref LocationOwner);
        serializer.SerializeValue(ref Index);
        serializer.SerializeValue(ref SecondIndex);
    }
}