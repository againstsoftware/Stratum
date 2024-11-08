using System;
using System.Collections.Generic;

public class Ecosystem
{
    public TableCard LastPlant => _plants.Count > 0 ? _plants[^1] : null;
    public TableCard LastHerbivore => _herbivores.Count > 0 ? _herbivores[^1] : null;
    public TableCard LastCarnivore => _carnivores.Count > 0 ? _carnivores[^1] : null;

    public IReadOnlyList<TableCard> Plants => _plants;
    public IReadOnlyList<TableCard> Herbivores => _herbivores;
    public IReadOnlyList<TableCard> Carnivores => _carnivores;

    private readonly List<TableCard> _plants = new(), _herbivores = new(), _carnivores = new();

    private int _totalPopulationCards;

    internal void OnPopulationCardPlace(TableCard tableCard)
    {
        if (tableCard.Card.CardType is not ICard.Card.Population) throw new Exception("Peticion invalida tal");

        foreach (var type in tableCard.GetPopulations())
        {
            switch (type)
            {
                case ICard.Population.Plant:
                    _plants.Add(tableCard);
                    break;
                case ICard.Population.Herbivore:
                    _herbivores.Add(tableCard);
                    break;
                case ICard.Population.Carnivore:
                    _carnivores.Add(tableCard);
                    break;
                default:
                    throw new Exception("Peticion invalida tal");
            }
        }

        _totalPopulationCards++;
    }

    internal void OnPopulationCardDie(TableCard tableCard)
    {
        if (tableCard.Card.CardType is not ICard.Card.Population) throw new Exception("Peticion invalida tal");

        foreach (var type in tableCard.GetPopulations())
        {
            switch (type)
            {
                case ICard.Population.Plant:
                    _plants.Remove(tableCard);
                    break;
                case ICard.Population.Herbivore:
                    _herbivores.Remove(tableCard);
                    break;
                case ICard.Population.Carnivore:
                    _carnivores.Remove(tableCard);
                    break;
                default:
                    throw new Exception("Peticion invalida tal");
            }
        }

        _totalPopulationCards--;
    }
}