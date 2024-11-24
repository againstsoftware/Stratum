using System;
using System.Collections.Generic;

public struct PlayerAction : IEquatable<PlayerAction>
{
    public PlayerCharacter Actor;
    public AActionItem ActionItem;
    public Receiver[] Receivers;
    public int EffectsIndex;

    public PlayerAction(PlayerCharacter actor, AActionItem actionItem, Receiver[] receivers, int effectsIndex)
    {
        Actor = actor;
        ActionItem = actionItem;
        Receivers = receivers;
        EffectsIndex = effectsIndex;
    }
    
    // Sobrescribir Equals
    public override bool Equals(object obj)
    {
        return obj is PlayerAction other && Equals(other);
    }

    public bool Equals(PlayerAction other)
    {
        // Comparar Receivers elemento por elemento
        if (Receivers.Length != other.Receivers.Length)
            return false;

        for (int i = 0; i < Receivers.Length; i++)
        {
            if (!Receivers[i].Equals(other.Receivers[i]))
                return false;
        }

        return EqualityComparer<PlayerCharacter>.Default.Equals(Actor, other.Actor)
               && EqualityComparer<AActionItem>.Default.Equals(ActionItem, other.ActionItem);
    }

    // Sobrescribir GetHashCode
    public override int GetHashCode()
    {
        int hash = HashCode.Combine(Actor, ActionItem);
        foreach (var receiver in Receivers)
        {
            hash = HashCode.Combine(hash, receiver);
        }
        return hash;
    }

}
