using System;
using System.Collections.Generic;

public class Ecosystem
{
    public TableCard LastPlant => _plants.Count > 0 ? _plants[^1] : null;
    public TableCard LastHerbivore => _herbivores.Count > 0 ? _herbivores[^1] : null;
    public TableCard LastCarnivore => _carnivores.Count > 0 ? _carnivores[^1] : null;

    private readonly List<TableCard> _plants = new(), _herbivores = new(), _carnivores = new();

    private int _totalPopulationCards;

    internal void OnPopulationCardPlace(TableCard card)
    {
        if (card.Card is not PopulationCard pc) throw new Exception("Peticion invalida tal");

        foreach (var type in pc.GetTypes())
        {
            switch (type)
            {
                case PopulationCard.Type.Plant:
                    _plants.Add(card);
                    break;
                case PopulationCard.Type.Herbivore:
                    _herbivores.Add(card);
                    break;
                case PopulationCard.Type.Carnivore:
                    _carnivores.Add(card);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _totalPopulationCards++;
    }

    internal void OnPopulationCardDie(TableCard card)
    {
        if (card.Card is not PopulationCard pc) throw new Exception("Peticion invalida tal");

        foreach (var type in pc.GetTypes())
        {
            switch (type)
            {
                case PopulationCard.Type.Plant:
                    _plants.Remove(card);
                    break;
                case PopulationCard.Type.Herbivore:
                    _herbivores.Remove(card);
                    break;
                case PopulationCard.Type.Carnivore:
                    _carnivores.Remove(card);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _totalPopulationCards++;
    }
}