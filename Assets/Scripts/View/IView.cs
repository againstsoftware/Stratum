using System;
using System.Collections.Generic;

public interface IView : IService
{
    public ViewPlayer GetViewPlayer(PlayerCharacter character);
    
    public void PlayCardOnSlot(ACard card, PlayerCharacter actor, PlayerCharacter slotOwner, int slotIndex, Action callback);
    

    public void GrowPopulationCard(PlayerCharacter slotOwner, int slotIndex, Action callback);
    
    public void KillPopulationCard(PlayerCharacter slotOwner, int slotIndex, Action callback);

    public void Discard(PlayerCharacter actor, Action callback);
    public void DrawCards(PlayerCharacter actor, IReadOnlyList<ACard> cards, Action callback);

    public void SwitchCamToOverview(Action callback);
}
