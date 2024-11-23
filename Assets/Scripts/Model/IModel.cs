using System;
using System.Collections.Generic;

public interface IModel : IService
{
    public PlayerCharacter PlayerOnTurn { get; }
    public bool IsOnEcosystemTurn { get; }
    
    public Ecosystem Ecosystem { get; }
    
    public GameConfig Config { get; }
    public Player GetPlayer(PlayerCharacter character);

    public int NumberOfConstructions { get; }

    public event Action<TableCard, TableCard> OnPopulationGrow;
    public event Action<TableCard> OnPopulationDie;
    public event Action OnCardPlaced, OnCardRemoved;

    

    public void RemoveCardFromHand(PlayerCharacter player, ACard card);
    public void PlaceCardOnSlot(ACard card, PlayerCharacter slotOwner, int slotIndex, bool atTheBottom = false);
    public void PlaceCardOnSlot(ACard card, Slot slot, bool atTheBottom = false);

    public void GrowLastPlacedPopulation(Population population, out TableCard parent, out TableCard child);
    public void GrowPopulation(TableCard parent, out TableCard child);
    public TableCard KillLastPlacedPopulation(Population population);

    public TableCard GrowMushroomOverLastDeadPopulation();
    public TableCard GrowMushroom(PlayerCharacter slotOwner, int slotIndex);
    public TableCard GrowMushroom(Slot slot);
    
    public void PlaceInlfuenceCardOnCard(ACard influenceCard, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex);

    public void PlaceInlfuenceCardOnCard(ACard influenceCard, TableCard tableCard);

    public void MoveCardBetweenSlots(PlayerCharacter slotOwner, int slotIndex, int cardIndex,
        PlayerCharacter targetSlotOwner, int targetSlotIndex);

    public void MoveCardBetweenSlots(TableCard card, Slot target);

    public void RemoveCardFromSlot(/*ACard card, */PlayerCharacter slotOwner, int slotIndex, int cardIndex);
    public void RemoveCardFromSlot(TableCard card);

    public void RemoveInfluenceCardFromCard(ACard card, PlayerCharacter slotOwner, int slotIndex, int cardIndex);
    public void RemoveInfluenceCardFromCard(TableCard cardWherePlaced);


    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromSlot(PlayerCharacter slotOwner, int slotIndex, Predicate<TableCard> filter = null);
    public void RemoveCardsFromSlot(Slot slot, Predicate<TableCard> filter = null);

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromTerritory(PlayerCharacter owner, Predicate<TableCard> filter = null);
    
    public void PlaceConstruction(PlayerCharacter territoryOwner, out TableCard plant1, out TableCard plant2);
    
    public void RemoveConstruction(PlayerCharacter territoryOwner);
    
    public void GiveRabies(PlayerCharacter slotOwner, int slotIndex, int cardIndex);
    public void MakeOmnivore(PlayerCharacter slotOwner, int slotIndex, int cardIndex); //NO
    public void PutLeash(PlayerCharacter slotOwner, int slotIndex, int cardIndex);
    
    public TableCard GetLastMushroomInTerritory(PlayerCharacter owner);
    
    public void AdvanceTurn(PlayerCharacter playerOnTurn);


    public IReadOnlyList<ACard> PlayerDrawCards(PlayerCharacter character/*, int amount*/);
}