using System;
using System.Collections.Generic;

public interface IView : IService
{
    public struct CardLocation
    {
        public PlayerCharacter SlotOwner;
        public int SlotIndex;
        public int CardIndex;
    }
    public ViewPlayer GetViewPlayer(PlayerCharacter character);
    public void PlayCardOnSlot(ACard card, PlayerCharacter actor, CardLocation location, Action callback);
    public void GrowPopulationCard(CardLocation location, Action callback);
    public void KillPopulationCard(CardLocation location, Action callback);
    public void Discard(PlayerCharacter actor, Action callback);
    public void DrawCards(PlayerCharacter actor, IReadOnlyList<ACard> cards, Action callback);
    public void SwitchCamToOverview(Action callback);
    public void GrowMushroom(CardLocation location, Action callback);

    public void GrowMacrofungi(CardLocation[] locations, Action callback);

    public void PlaceConstruction(CardLocation plant1Location, CardLocation plant2Location, Action callback);
}
