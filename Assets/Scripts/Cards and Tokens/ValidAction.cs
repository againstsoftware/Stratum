using System;
using UnityEngine;

[Serializable]
public class ValidAction
{
    [field: SerializeField] public ValidDropLocation DropLocation { get; private set; }
    [field: SerializeField] public ValidActionReceiver[] Receivers { get; private set; }
}

public enum ValidDropLocation
{
    OwnerSlot,
    AnySlot,
    AnyTerritory,
    OwnerCard,
    AnyCard,
    TableCenter
}

public enum ValidActionReceiver
{
    OwnerSlot,
    AnySlot,
    AnyTerritory,
    OwnerCard,
    AnyCard
}


