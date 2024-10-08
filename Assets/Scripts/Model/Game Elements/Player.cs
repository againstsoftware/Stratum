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

        for (int i = 0; i < 5; i++) DrawCard();
    }

    internal void AdvanceTurn()
    {
        Territory.AdvanceTurn();
    }

    internal void DrawCard()
    {
        var drewCard = Deck.DrawCard();
        HandOfCards.AddCard(drewCard);
    }
}

public enum PlayerCharacter
{
    Ygdra, Sagitario, Fungaloth, Overlord, None
}