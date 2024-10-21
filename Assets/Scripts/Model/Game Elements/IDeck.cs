
public interface IDeck
{
    public int Size { get; }
    public PlayerCharacter Owner { get; }
    public ICard DrawCard();

    //para cartas que NO esten en el mazo
    public ICard Mushroom { get; }
    public ICard Macrofungi { get; }
}
