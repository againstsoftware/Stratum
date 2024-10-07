using System;

public interface IView : IService
{
    public void PlayCardOnSlot(PlayerCharacter actor, ACard card, int cardIndex, int slotIndex, Action callback);
}
