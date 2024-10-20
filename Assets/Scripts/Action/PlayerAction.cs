using System.Collections.Generic;

public struct PlayerAction
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

}
