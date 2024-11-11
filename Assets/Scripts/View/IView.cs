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

    public void PlaceInitialCards(IReadOnlyList<(ACard card, CardLocation location)> cardsAndLocations,
        Action callback);

    public void PlaceCardOnSlotFromDeck(ACard card, CardLocation location, Action callback);
    public void GrowPopulationCardEcosystem(CardLocation location, Action callback);
    public void KillPopulationCardEcosystem(CardLocation location, Action callback);
    public void Discard(PlayerCharacter actor, Action callback);
    public void DrawCards(IReadOnlyDictionary<PlayerCharacter, IReadOnlyList<ACard>> cardsDrawn, Action callback);
    public void SwitchCamToOverview(Action callback);

    public void GrowPopulation(CardLocation location, Population population,
        Action callback, bool isEndOfAction = false);
    public void GrowMushroom(CardLocation location, Action callback, bool isEndOfAction = false);

    public void GrowMacrofungi(CardLocation[] locations, Action callback);

    public void PlaceConstruction(CardLocation plant1Location, CardLocation plant2Location, Action callback);

    public void PlayAndDiscardInfluenceCard(PlayerCharacter actor, AInfluenceCard card, CardLocation location,
        Action callback, bool isEndOfAction = false);

    public void MovePopulationToEmptySlot(PlayerCharacter actor, CardLocation from, CardLocation to, Action callback);

    public void PlaceInfluenceOnPopulation(PlayerCharacter actor, AInfluenceCard influenceCard, CardLocation location,
        Action callback, bool isEndOfAction = false);

    public void GiveRabies(PlayerCharacter actor, CardLocation location, Action callback);

    public void MakeOmnivore(PlayerCharacter actor, CardLocation location, Action callback);

    public void DestroyInTerritory(PlayerCharacter actor, PlayerCharacter territoryOwner, Action callback,
        Predicate<ACard> filter = null);

    public void DestroyConstruction(PlayerCharacter territoryOwner, Action callback);
    public void KillPlacedCard(CardLocation location, Action callback);

    public void DiscardInfluenceFromPopulation(CardLocation location, Action callback);
}