
public interface IDeck
{
    public int Size { get; }
    public PlayerCharacter Owner { get; }
    public ICard DrawCard();
}
