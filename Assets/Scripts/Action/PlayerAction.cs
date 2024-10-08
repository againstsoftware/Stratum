
using System.Collections.Generic;

public struct PlayerAction
{
    public readonly PlayerCharacter Actor;
    public readonly IActionItem ActionItem;
    public readonly IReadOnlyList<Receiver> Receivers;
    public readonly int CardIndexInHand;

    public PlayerAction(PlayerCharacter actor, IActionItem actionItem, IReadOnlyList<Receiver> receivers,
        int cardIndexInHand)
    {
        Actor = actor;
        ActionItem = actionItem;
        Receivers = receivers;
        CardIndexInHand = cardIndexInHand;
    }
}
