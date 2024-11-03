using System;
using System.Collections.Generic;

public interface IModel : IService
{
    public PlayerCharacter PlayerOnTurn { get; }
    public bool IsOnEcosystemTurn { get; }
    
    public Ecosystem Ecosystem { get; }
    
    
    public Player GetPlayer(PlayerCharacter character);


    public event Action<TableCard> OnPopulationGrow;
    public event Action<TableCard> OnPopulationDie;

    public void RemoveCardFromHand(PlayerCharacter player, ICard card);
    public void PlaceCardOnSlot(ICard card, PlayerCharacter slotOwner, int slotIndex, bool atTheBottom = false);
    public (TableCard parent, TableCard son) GrowLastPlacedPopulation(ICard.Population population);
    public TableCard KillLastPlacedPopulation(ICard.Population population);

    public TableCard GrowMushroom();
    public TableCard GrowMushroom(PlayerCharacter slotOwner, int slotIndex);

    public void PlaceInlfuenceCardOnCard(ICard influenceCard, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex);

    public void MoveCardBetweenSlots(PlayerCharacter slotOwner, int slotIndex, int cardIndex,
        PlayerCharacter targetSlotOwner, int targetSlotIndex);

    public void RemoveCardFromSlot(/*ICard card, */PlayerCharacter slotOwner, int slotIndex, int cardIndex);

    public void RemoveInfluenceCardFromCard(ICard influenceCard, ICard card, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex);

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromSlot(PlayerCharacter slotOwner, int slotIndex, Predicate<TableCard> filter = null);

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromTerritory(PlayerCharacter owner, Predicate<TableCard> filter = null);
    
    public void PlaceConstruction(PlayerCharacter territoryOwner, out TableCard plant1, out TableCard plant2);
    
    public void RemoveConstruction(PlayerCharacter territoryOwner);
    
    public void GiveRabies(PlayerCharacter slotOwner, int slotIndex, int cardIndex);
    public void AdvanceTurn(PlayerCharacter playerOnTurn);


    public IReadOnlyList<ICard> PlayerDrawCards(PlayerCharacter character/*, int amount*/);
}