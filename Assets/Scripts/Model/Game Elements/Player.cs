public class Player
{
    public readonly PlayerCharacter Character;
    public readonly Deck Deck;
    public readonly HandOfCards HandOfCards;
    public readonly Territory Territory;
    
    public bool TokenPlayed { get; set; }
    public bool InfluencePlayed { get; set; }

    internal Player(PlayerCharacter character, Deck deck)
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

    internal ACard DrawCard()
    {
        var drewCard = Deck.DrawCard();
        HandOfCards.AddCard(drewCard);
        return drewCard;
    }

    internal ACard DrawFixedCard(ACard card)
    {
        HandOfCards.AddCard(card);
        return card;
    }
}

