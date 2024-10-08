using System;

public interface IView : IService
{
    public ViewPlayer GetViewPlayer(PlayerCharacter character);
    
    public void PlayCardOnSlot(PlayerCharacter actor, ACard card, int cardIndex, int slotIndex, Action callback);
}
