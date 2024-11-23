using System;
using System.Collections.Generic;
using Unity.Netcode;

public struct Receiver : INetworkSerializable, IEquatable<Receiver>
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
    
    // Sobrescribir Equals
    public override bool Equals(object obj)
    {
        return obj is Receiver other && Equals(other);
    }

    public bool Equals(Receiver other)
    {
        return Location.Equals(other.Location)
               && EqualityComparer<PlayerCharacter>.Default.Equals(LocationOwner, other.LocationOwner)
               && Index == other.Index
               && SecondIndex == other.SecondIndex;
    }

    // Sobrescribir GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(Location, LocationOwner, Index, SecondIndex);
    }
}