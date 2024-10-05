using System;
using System.Collections.Generic;

public interface IModel : IService
{
    public PlayerCharacter PlayerOnTurn { get; }
    public bool IsOnEcosystemTurn { get; }
    
    public Ecosystem Ecosystem { get; }
    
    
    public Player GetPlayer(PlayerCharacter character);
    
    
    
    public void PlaceCardOnSlot(ACard card, PlayerCharacter slotOwner, int slotIndex, bool atTheBottom = false);

    public void PlaceInlfuenceCardOnCard(InfluenceCard influenceCard, ACard card, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex);

    public void MoveCardBetweenSlots(ACard card, PlayerCharacter slotOwner, int slotIndex, int cardIndex,
        PlayerCharacter targetSlotOwner, int targetSlotIndex);

    public void RemoveCardFromSlot(ACard card, PlayerCharacter slotOwner, int slotIndex, int cardIndex);

    public void RemoveInfluenceCardFromCard(InfluenceCard influenceCard, ACard card, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex);

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromSlot(PlayerCharacter slotOwner, int slotIndex, Predicate<TableCard> filter = null);

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromTerritory(PlayerCharacter owner, Predicate<TableCard> filter = null);
    
    public void PlaceConstruction(PlayerCharacter territoryOwner);
    
    public void RemoveConstruction(PlayerCharacter territoryOwner);
    
    public void AdvanceTurn(PlayerCharacter playerOnTurn);
    
    public void AdvanceTurnToEcosystem();
    
    public void PlayerDrawCards(PlayerCharacter character, int amount);
}