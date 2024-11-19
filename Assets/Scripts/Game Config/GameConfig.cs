using System;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Game Config")]    
public class GameConfig : ScriptableObject
{
    [field:SerializeField] public int ActionsPerTurn { get; private set; }
    
    [field:SerializeField] public PlayerCharacter[] TurnOrder { get; private set; }
    [field:SerializeField] public GameObject CardPrefab { get; private set; }
    [field:SerializeField] public GameObject ConstructionPrefab { get; private set; }
    
    [field:SerializeField] public MushroomCard Mushroom { get; private set; }
    [field:SerializeField] public MacrofungiCard Macrofungi { get; private set; }
    
    [field:SerializeField] public PopulationCard[] InitialCards { get; private set; }

    [SerializeField] private AActionItem[] _actionItems;

    [SerializeField] private PopulationCard[] _populationCards;
    
    // [field: SerializeField] public int NaturePlantsToWin { get; private set; } 
    // [field: SerializeField] public int NatureHerbivoresToWin { get; private set; } 
    // [field: SerializeField] public int NatureCarnivoresToWin { get; private set; } 
    
    [field: SerializeField] public int GrowthsToWin { get; private set; } 
    [field: SerializeField] public int MacrofungiToWin { get; private set; } 
    [field: SerializeField] public int ConstructionsToWin { get; private set; } 

    public int ActionItemToID(AActionItem actionItem) => Array.IndexOf(_actionItems, actionItem);
    public AActionItem IDToActionItem(int id) => _actionItems[id];

    public PopulationCard GetPopulationCard(Population p) =>
        _populationCards.FirstOrDefault(pc => pc.PopulationType == p);
}
