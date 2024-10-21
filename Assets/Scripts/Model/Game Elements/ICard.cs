
using System.Collections.Generic;

public interface ICard
{
    public enum Card { Population, Influence, Mushroom, Macrofungi }
    public enum Population { None, Carnivore, Herbivore, Plant}
    
    public Card CardType { get; }
    public IEnumerable<Population> GetPopulations();
    public bool CanHaveInfluenceCardOnTop { get; }

}
