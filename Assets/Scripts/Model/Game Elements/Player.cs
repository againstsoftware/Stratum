public class Player
{
    public readonly PlayerCharacter Character;
    public readonly IDeck Deck;
    public readonly HandOfCards HandOfCards;
    public readonly Territory Territory;
    
    public bool TokenPlayed { get; set; }
    public bool InfluencePlayed { get; set; }

    internal Player(PlayerCharacter character, IDeck deck)
    {
        Character = character;
        Deck = deck;
        HandOfCards = new();
        Territory = new(Character);
        
    }

    internal void AdvanceTurn()
    {
        TokenPlayed = false;
        InfluencePlayed = false;
        
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