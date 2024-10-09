using System.Collections.Generic;

public struct PlayerAction
{
    public PlayerCharacter Actor;
    public AActionItem ActionItem;
    public Receiver[] Receivers;
    public int CardIndexInHand;

    public PlayerAction(PlayerCharacter actor, AActionItem actionItem, Receiver[] receivers,
        int cardIndexInHand)
    {
        Actor = actor;
        ActionItem = actionItem;
        Receivers = receivers;
        CardIndexInHand = cardIndexInHand;
    }

}
