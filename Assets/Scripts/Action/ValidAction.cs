using System;
using UnityEngine;

[Serializable]
public class ValidAction
{
    [field: SerializeField] public ValidDropLocation DropLocation { get; private set; }
    [field: SerializeField] public ValidActionReceiver[] Receivers { get; private set; }

    public ValidAction(ValidDropLocation dropLocation, ValidActionReceiver[] receivers = null)
    {
        DropLocation = dropLocation;
        Receivers = receivers;
    }

    public int Index { get; set; } = 0;

    public static ValidDropLocation ActionReceiverToDropLocation(ValidActionReceiver r) => r switch
    {
        ValidActionReceiver.OwnerSlot => ValidDropLocation.OwnerSlot,
        ValidActionReceiver.AnySlot => ValidDropLocation.AnySlot,
        ValidActionReceiver.AnyTerritory => ValidDropLocation.AnyTerritory,
        ValidActionReceiver.OwnerCard => ValidDropLocation.OwnerCard,
        ValidActionReceiver.AnyCard => ValidDropLocation.AnyCard,
        _ => throw new ArgumentOutOfRangeException()
    };
}

public enum ValidDropLocation
{
    OwnerSlot,
    AnySlot,
    AnyTerritory,
    OwnerCard,
    AnyCard,
    TableCenter,
    DiscardPile
}

public enum ValidActionReceiver
{
    OwnerSlot,
    AnySlot,
    AnyTerritory,
    OwnerCard,
    AnyCard,
}




