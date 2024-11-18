using System;
using System.Collections.Generic;
using System.Linq;

public class Ecosystem
{
    public TableCard LastPlant => _plants.Count > 0 ? _plants[^1] : null;
    public TableCard LastHerbivore => _herbivores.Count > 0 ? _herbivores[^1] : null;
    public TableCard LastCarnivore => _carnivores.Count > 0 ? _carnivores[^1] : null;

    public IReadOnlyList<TableCard> Plants => _plants;
    public IReadOnlyList<TableCard> Herbivores => _herbivores;
    public IReadOnlyList<TableCard> Carnivores => _carnivores;
    public IReadOnlyList<TableCard> Mushrooms => _mushrooms;
    public IReadOnlyList<TableCard> Macrofungi => _macrofungi;

    public event Action OnEcosystemChange;

    private readonly List<TableCard> _plants = new(),
        _herbivores = new(),
        _carnivores = new(),
        _mushrooms = new(),
        _macrofungi = new();

    private int _totalPopulationCards;

    internal void OnPopulationCardPlace(TableCard tableCard)
    {
        if (tableCard.Card is not PopulationCard) throw new Exception("Peticion invalida tal");

        if (tableCard.Slot.Territory.HasConstruction) return;

        foreach (var type in tableCard.GetPopulations())
        {
            switch (type)
            {
                case Population.Plant:
                    _plants.Add(tableCard);
                    break;
                case Population.Herbivore:
                    _herbivores.Add(tableCard);
                    break;
                case Population.Carnivore:
                    _carnivores.Add(tableCard);
                    break;
                default:
                    throw new Exception("Peticion invalida tal");
            }
        }

        _totalPopulationCards++;
        OnEcosystemChange?.Invoke();
    }

    internal void OnMushroomCardPlace(TableCard mushroom)
    {
        _mushrooms.Add(mushroom);
    }

    internal void OnMacrofungiCardPlace(TableCard mushroom)
    {
        _macrofungi.Add(mushroom);
    }

    internal void OnPopulationCardDie(TableCard tableCard)
    {
        if (tableCard.Card is not PopulationCard) throw new Exception("Peticion invalida tal");

        if (tableCard.Slot.Territory.HasConstruction) return;

        RemovePopulation(tableCard);
    }

    private void RemovePopulation(TableCard tableCard)
    {
        foreach (var type in tableCard.GetPopulations())
        {
            switch (type)
            {
                case Population.Plant:
                    _plants.Remove(tableCard);
                    break;
                case Population.Herbivore:
                    _herbivores.Remove(tableCard);
                    break;
                case Population.Carnivore:
                    _carnivores.Remove(tableCard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _totalPopulationCards--;
        OnEcosystemChange?.Invoke();
    }

    internal void OnMushroomCardDie(TableCard mushroom)
    {
        _mushrooms.Remove(mushroom);
    }

    internal void OnMacrofungiCardDie(TableCard mushroom)
    {
        _macrofungi.Remove(mushroom);
    }

    internal void OnConstructionPlaced(Territory territory)
    {
        foreach (var slot in territory.Slots)
        {
            foreach (var tableCard in slot.PlacedCards)
            {
                //VAMOS A ASUMIR QUE HAY 1 POBLACION POR CARTA
                switch (tableCard.GetPopulations().FirstOrDefault())
                {
                    case Population.Plant:
                        if (!_plants.Remove(tableCard))
                            throw new Exception("error quitando poblaciones del registro del ecosistema");
                        break;
                    case Population.Herbivore:
                        if (!_herbivores.Remove(tableCard))
                            throw new Exception("error quitando poblaciones del registro del ecosistema");
                        break;
                    case Population.Carnivore:
                        if (!_carnivores.Remove(tableCard))
                            throw new Exception("error quitando poblaciones del registro del ecosistema");
                        break;
                    case Population.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _totalPopulationCards--;
            }
        }
        OnEcosystemChange?.Invoke();
    }

    internal void OnConstructionRemoved(Territory territory)
    {
        foreach (var slot in territory.Slots)
        {
            foreach (var tableCard in slot.PlacedCards)
            {
                //VAMOS A ASUMIR QUE HAY 1 POBLACION POR CARTA
                switch (tableCard.GetPopulations().FirstOrDefault())
                {
                    case Population.Plant:
                        _plants.Add(tableCard);
                        break;
                    case Population.Herbivore:
                        _herbivores.Add(tableCard);
                        break;
                    case Population.Carnivore:
                        _carnivores.Add(tableCard);
                        break;
                    case Population.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _totalPopulationCards++;
            }
        }
        OnEcosystemChange?.Invoke();
    }

    internal void OnPopulationMoved(TableCard tableCard, Slot oldSlot)
    {
        var newSlot = tableCard.Slot;
        if (oldSlot.Territory.HasConstruction && !newSlot.Territory.HasConstruction)
        {
            OnPopulationCardPlace(tableCard);
        }
        else if (!oldSlot.Territory.HasConstruction && newSlot.Territory.HasConstruction)
        {
            RemovePopulation(tableCard);
        }
    }
}