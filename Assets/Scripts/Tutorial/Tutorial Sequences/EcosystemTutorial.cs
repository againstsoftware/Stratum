using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "Tutorials/Ecosystem Tutorial")]
public class EcosystemTutorial : ATutorialSequence
{
    [field: SerializeField] public override PlayerCharacter LocalPlayer { get; protected set; }

    [SerializeField] private TutorialDialogue[] _initialDialogues;

    //accion de introducir demasiados herbivoros
    [SerializeField] private TutorialDialogue[] _dialoguesHerbivoresPlantsDie;

    //accion de turno de ecosistema herb muere
    [SerializeField] private TutorialDialogue[] _dialoguesMushroom;

    //accion de poner mas plantas
    [SerializeField] private TutorialDialogue[] _dialoguesHerbivoresPlantsGrow;

    //turno de ecosistema herb crece
    [SerializeField] private TutorialDialogue[] _dialoguesHerbivoresPlantsAfter;

    //se quitan las cartas y se ponen otras para herbs carns
    [SerializeField] private TutorialDialogue[] _dialoguesCarnivoresHerbivoresDie;

    //turno de ecosistema herb muere carn muere
    [SerializeField] private TutorialDialogue[] _dialoguesCarnHerbDieAfter;

    //se agregan plantas pero aun asi palmaran
    [SerializeField] private TutorialDialogue[] _plantsCarnsHerbsDie;

    //turno de ecosistema mueren herbs
    [SerializeField] private TutorialDialogue[] _plantsCarnsHerbsDieAfter;

    //se quitan las cartas y ponen otras para herbs carns crecer
    [SerializeField] private TutorialDialogue[] _dialoguesCarnHerbGrow;

    //turno del ecosistema crecen ambos
    [SerializeField] private TutorialDialogue[] _dialoguesCarnHerbGrowAfter;


    public override IEnumerable<ITutorialElement> GetTutorialElements()
    {
        List<ITutorialElement> elements = new();

        var config = ServiceLocator.Get<IModel>().Config;
        var players = new List<PlayerCharacter>(config.TurnOrder);
        players.Remove(PlayerCharacter.None);
        int indexOfLocalPlayer = players.IndexOf(LocalPlayer);
        int oppositeIndex = (indexOfLocalPlayer + 2) % players.Count;
        var oppositePlayer = players[oppositeIndex];
        int leftIndex = (indexOfLocalPlayer + 3) % players.Count;
        var leftPlayer = players[leftIndex];


        var plant = config.GetPopulationCard(Population.Plant);
        var herbivore = config.GetPopulationCard(Population.Herbivore);
        var carnivore = config.GetPopulationCard(Population.Carnivore);
        var playerTerritory = ServiceLocator.Get<IModel>().GetPlayer(LocalPlayer).Territory;
        var oppositeTerritory = ServiceLocator.Get<IModel>().GetPlayer(oppositePlayer).Territory;
        var leftTerritory = ServiceLocator.Get<IModel>().GetPlayer(leftPlayer).Territory;

        elements.AddRange(_initialDialogues);

        var placeHerbsPlants = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, oppositeTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, oppositeTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, oppositeTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, playerTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, playerTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, playerTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, playerTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, playerTerritory.Slots[4]),
        });

        elements.Add(placeHerbsPlants);

        elements.AddRange(_dialoguesHerbivoresPlantsDie);

        elements.Add(EcosystemAct());

        elements.AddRange(_dialoguesMushroom);

        var placeMorePlants = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, oppositeTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, oppositeTerritory.Slots[4]),
            new EffectCommands.RemoveCardFromSlotTutorial(playerTerritory.Slots[4], 0),
            new EffectCommands.RemoveCardFromSlotTutorial(playerTerritory.Slots[0], 0),


        });
        elements.Add(placeMorePlants);

        elements.AddRange(_dialoguesHerbivoresPlantsGrow);

        elements.Add(EcosystemAct());

        elements.AddRange(_dialoguesHerbivoresPlantsAfter);

        var removeInTerritoryAndPlaceCarnsAndHerbs = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(playerTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(oppositeTerritory),
            
            new EffectCommands.PlaceCardOnSlotTutorial(plant, leftTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, leftTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, leftTerritory.Slots[3]),
            
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, oppositeTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, oppositeTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, oppositeTerritory.Slots[3]),
            
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, playerTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, playerTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, playerTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, playerTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, playerTerritory.Slots[4]),
        });
        
        elements.Add(removeInTerritoryAndPlaceCarnsAndHerbs);
        
        elements.AddRange(_dialoguesCarnivoresHerbivoresDie);
        
        elements.Add(EcosystemAct());
        
        elements.AddRange(_dialoguesCarnHerbDieAfter);

        var addPlant = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, leftTerritory.Slots[4]),
        });
        
        elements.Add(addPlant);
        
        elements.AddRange(_plantsCarnsHerbsDie);
        
        elements.Add(EcosystemAct());
        
        elements.AddRange(_plantsCarnsHerbsDieAfter);

        var removePlantRemoveCarnAddHerbs = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.RemoveCardFromSlotTutorial(leftTerritory.Slots[4], 0),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(oppositeTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(playerTerritory),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, playerTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, oppositeTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, oppositeTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, oppositeTerritory.Slots[3]),
        });
        
        elements.Add(removePlantRemoveCarnAddHerbs);
        
        elements.AddRange(_dialoguesCarnHerbGrow);
        
        elements.Add(EcosystemAct());
        
        elements.AddRange(_dialoguesCarnHerbGrowAfter);

        return elements;
    }


    public override void OnTutorialFinished()
    {
        
    }

    private TutorialAction EcosystemAct()
    {
        return new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.RushEcosystemTurn(),
        });
    }
}