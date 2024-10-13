public class Player
{
    public readonly PlayerCharacter Character;
    public readonly IDeck Deck;
    public readonly HandOfCards HandOfCards;
    public readonly Territory Territory;

    internal Player(PlayerCharacter character, IDeck deck)
    {
        Character = character;
        Deck = deck;
        HandOfCards = new();
        Territory = new(Character);
        
    }

    internal void AdvanceTurn()
    {
        Territory.AdvanceTurn();
    }

    internal ICard DrawCard()
    {
        var drewCard = Deck.DrawCard();
        HandOfCards.AddCard(drewCard);
        return drewCard;
    }
}

public enum PlayerCharacter
{
    Ygdra, Sagitario, Fungaloth, Overlord, None
}