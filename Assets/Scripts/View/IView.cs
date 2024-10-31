using System;
using System.Collections.Generic;

public interface IView : IService
{
    public struct CardLocation
    {
        public PlayerCharacter Owner;
        public int SlotIndex;
        public int CardIndex;
        public bool IsTerritory;
    }
    public ViewPlayer GetViewPlayer(PlayerCharacter character);
    public void PlayCardOnSlotFromPlayer(ACard card, PlayerCharacter actor, CardLocation location, Action callback);
    public void PlaceInitialCards(IReadOnlyList<(ACard card, CardLocation location)> cardsAndLocations, Action callback);
    public void PlaceCardOnSlotFromDeck(ACard card, CardLocation location, Action callback);
    public void GrowPopulationCard(CardLocation location, Action callback);
    public void KillPopulationCard(CardLocation location, Action callback);
    public void Discard(PlayerCharacter actor, Action callback);
    public void DrawCards(IReadOnlyDictionary<PlayerCharacter, IReadOnlyList<ACard>> cardsDrawn, Action callback);
    public void SwitchCamToOverview(Action callback);
    public void GrowMushroom(CardLocation location, Action callback);

    public void GrowMacrofungi(CardLocation[] locations, Action callback);

    public void PlaceConstruction(CardLocation plant1Location, CardLocation plant2Location, Action callback);

    public void PlayAndDiscardInfluenceCard(PlayerCharacter actor, CardLocation location, Action callback);
    public void MovePopulationToEmptySlot(CardLocation from, CardLocation to, Action callback);
}
