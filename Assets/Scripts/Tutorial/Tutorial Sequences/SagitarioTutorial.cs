using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Tutorials/Sagitario Tutorial")]
public class SagitarioTutorial : ATutorialSequence
{
    public override PlayerCharacter LocalPlayer { get; protected set; } = PlayerCharacter.Sagitario;

    [SerializeField] private List<ACard> _initialCards;
    [SerializeField] private List<ACard> _rabiesCards;
    [SerializeField] private List<ACard> _migrationCards;
    [SerializeField] private List<ACard> _mushroomPredCards;
    [SerializeField] private List<ACard> _motherNatureCards;

    [SerializeField] private TutorialDialogue[] _initialDialogues;

    //roba las 5 cartas scripteadas
    [SerializeField] private TutorialDialogue[] _cardTutorialDialogues;

    //turno del jugador (2 acciones) para poner cartas de poblacion
    [SerializeField] private TutorialDialogue[] _fungalothTurnDialogues;

    //juega fungaloth
    [SerializeField] private TutorialDialogue[] _ygdraTurnDialogues;

    //juega ygdra
    [SerializeField] private TutorialDialogue[] _overlordTurnDialogues;

    //juega overlord
    [SerializeField] private TutorialDialogue[] _ecosystemTurnDialogues;

    //juega el ecosistema
    [SerializeField] private TutorialDialogue[] _roundEndDialogues;

    //se reparten 2 cartas de influencia y ponen cartas para jugar rabia
    [SerializeField] private TutorialDialogue[] _playRabies;

    //se juega la ronda hasta overlord, para explicar que no puede construir porque tal
    [SerializeField] private TutorialDialogue[] _overlordRabiesTurnDialogue;

    //ecosistema + robar + nuevas cartas para usar migracion
    [SerializeField] private TutorialDialogue[] _playMigration;

    //ronda full y colocar cartas pa jugar depredador de setas
    [SerializeField] private TutorialDialogue[] _playMushroomPredator;
    
    //ronda full yecosistema, pero antes de que se coma el hongo este
    [SerializeField] private TutorialDialogue[] _mushroomPredatorEffectDialogue;

    //se come el hongo, reparten cartas y colocar caartas pa jugar madre naturaleza
    [SerializeField] private TutorialDialogue[] _playMotherNature;

    //se aplica el efecto y ya despedida
    [SerializeField] private TutorialDialogue[] _outroDialogue;


    public override IEnumerable<ITutorialElement> GetTutorialElements()
    {
        List<ITutorialElement> elements = new();

        var config = ServiceLocator.Get<IModel>().Config;

        var plant = config.GetPopulationCard(Population.Plant);
        var herbivore = config.GetPopulationCard(Population.Herbivore);
        var carnivore = config.GetPopulationCard(Population.Carnivore);
        var mushroom = config.Mushroom;
        var sagitarioTerritory = ServiceLocator.Get<IModel>().GetPlayer(PlayerCharacter.Sagitario).Territory;
        var fungalothTerritory = ServiceLocator.Get<IModel>().GetPlayer(PlayerCharacter.Fungaloth).Territory;
        var ygdraTerritory = ServiceLocator.Get<IModel>().GetPlayer(PlayerCharacter.Ygdra).Territory;
        var overlordTerritory = ServiceLocator.Get<IModel>().GetPlayer(PlayerCharacter.Overlord).Territory;

        elements.AddRange(_initialDialogues);

        elements.Add(DrawFixed(_initialCards));

        elements.AddRange(_cardTutorialDialogues);

        //turno del jugador para jugar poblaciones        
        elements.Add(new TutorialAction(true));
        elements.Add(new TutorialAction(true));

        elements.AddRange(_fungalothTurnDialogues);

        var fungalothTurn1 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, fungalothTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, fungalothTerritory.Slots[2]),
        });

        elements.Add(fungalothTurn1);

        elements.AddRange(_ygdraTurnDialogues);

        var ygdraTurn1 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[0]),
        });

        elements.Add(ygdraTurn1);

        elements.AddRange(_overlordTurnDialogues);

        var overlordTurn1 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[4]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, overlordTerritory.Slots[0]),
        });

        elements.Add(overlordTurn1);

        elements.AddRange(_ecosystemTurnDialogues);


        elements.Add(EcosystemAct());

        elements.AddRange(_roundEndDialogues);

        elements.Add(DrawFixed(_rabiesCards));

        var wipeAndPlaceForRabies = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.RemoveCardsFromTerritoryTutorial(sagitarioTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(fungalothTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(ygdraTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(overlordTerritory),

            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, sagitarioTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, fungalothTerritory.Slots[0]),
        });

        elements.Add(wipeAndPlaceForRabies);

        elements.AddRange(_playRabies);

        //turno del jugador solo puede jugar rabia
        var sagHerbReceiver = new Receiver(ValidDropLocation.AnyCard, PlayerCharacter.Sagitario, 1, 0);
        var playRabiesOnSagHerb = new PlayerAction(PlayerCharacter.Sagitario, _rabiesCards[0],
            new Receiver[] { sagHerbReceiver }, 0);

        var ovHerbReceiver = new Receiver(ValidDropLocation.AnyCard, PlayerCharacter.Overlord, 3, 0);
        var playRabiesOnOvHerb = new PlayerAction(PlayerCharacter.Sagitario, _rabiesCards[0],
            new Receiver[] { ovHerbReceiver }, 0);

        elements.Add(new TutorialAction(true, null, new List<PlayerAction>()
        {
            playRabiesOnOvHerb, playRabiesOnSagHerb
        }));

        elements.Add(new TutorialAction(true));


        var fungalothTurn2 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, fungalothTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, fungalothTerritory.Slots[2]),
        });

        elements.Add(fungalothTurn2);

        var ygdraTurn2 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, ygdraTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[0]),
        });

        elements.Add(ygdraTurn2);

        elements.AddRange(_overlordRabiesTurnDialogue);

        var overlordTurn2 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[4]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[2]),
        });

        elements.Add(overlordTurn2);

        elements.Add(EcosystemAct());

        elements.Add(DrawFixed(_migrationCards));

        var wipeSagForMigration = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.RemoveCardsFromTerritoryTutorial(sagitarioTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(fungalothTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(ygdraTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(overlordTerritory),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, sagitarioTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, sagitarioTerritory.Slots[4]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, fungalothTerritory.Slots[2]),
        });

        elements.Add(wipeSagForMigration);

        elements.AddRange(_playMigration);

        var forcedMigrationAction = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Sagitario, _migrationCards[0], null, 1)
        };
        var playMigration = 
            new TutorialAction(true, null, forcedMigrationAction, true);

        elements.Add(playMigration);
        elements.Add(new TutorialAction(true));

        
        var fungalothTurn3 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, fungalothTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(carnivore, fungalothTerritory),
        });

        elements.Add(fungalothTurn3);

        var ygdraTurn3 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(herbivore, ygdraTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, ygdraTerritory),
        });

        elements.Add(ygdraTurn3);
        
        var overlordTurn3 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, overlordTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, overlordTerritory),
        });

        elements.Add(overlordTurn3);

        elements.Add(EcosystemAct());

        elements.Add(DrawFixed(_mushroomPredCards));
        
        
        
        var wipeSagForMushPred = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.RemoveCardsFromTerritoryTutorial(sagitarioTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(fungalothTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(ygdraTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(overlordTerritory),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, sagitarioTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, overlordTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, fungalothTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(mushroom, fungalothTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(mushroom, fungalothTerritory.Slots[3]),
        });

        elements.Add(wipeSagForMushPred);
        
        elements.AddRange(_playMushroomPredator);
        
        
        var forcedMushPredAction = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Sagitario, _mushroomPredCards[0], null, 1)
        };
        var playMushPred = 
            new TutorialAction(true, null, forcedMushPredAction, true);

        elements.Add(playMushPred);
        elements.Add(new TutorialAction(true));
        
        
        var fungalothTurn4 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, fungalothTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(carnivore, fungalothTerritory),
        });

        elements.Add(fungalothTurn4);

        var ygdraTurn4 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, ygdraTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, ygdraTerritory),
        });

        elements.Add(ygdraTurn4);


        var overlordTurn4 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(carnivore, overlordTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(herbivore, overlordTerritory),
        });

        elements.Add(overlordTurn4);

        elements.Add(EcosystemAct());
        
        elements.AddRange(_mushroomPredatorEffectDialogue);

        var mushPredEffect =
            new TutorialAction(false, ServiceLocator.Get<IRulesSystem>().GetRoundEndObserversEffects);
        elements.Add(mushPredEffect);

        elements.Add(DrawFixed(_motherNatureCards));
        
        
        var wipeSagForMushPredMotherN = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.RemoveCardsFromTerritoryTutorial(sagitarioTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(fungalothTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(ygdraTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(overlordTerritory),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, sagitarioTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, sagitarioTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, sagitarioTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, fungalothTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, fungalothTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, overlordTerritory.Slots[3]),
        });

        elements.Add(wipeSagForMushPredMotherN);
        
        elements.AddRange(_playMotherNature);
        
        var forcedMotherNAction = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Sagitario, _motherNatureCards[0], null, 1)
        };
        var playMotherN = 
            new TutorialAction(true, null, forcedMotherNAction, true);

        elements.Add(playMotherN);
        elements.Add(new TutorialAction(true));
        
        elements.AddRange(_outroDialogue);

        
        return elements;
    }

    public override void OnTutorialFinished()
    {
        SceneTransition.Instance.TransitionToCurrentScene();
    }
    


}