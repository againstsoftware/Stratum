public struct Receiver
{
    public readonly ValidDropLocation Location;
    public readonly PlayerCharacter LocationOwner;
    public readonly int Index, SecondIndex;

    public Receiver(ValidDropLocation validDropLocation, PlayerCharacter locationOwner, int index, int secondIndex)
    {
        Location = validDropLocation;
        LocationOwner = locationOwner;
        Index = index;
        SecondIndex = secondIndex;
    }
}