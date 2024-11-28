using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Tutorials/Ygdra Tutorial")]
public class YgdraTutorial : ATutorialSequence
{
    public override PlayerCharacter LocalPlayer { get; protected set; } = PlayerCharacter.Ygdra;

    [SerializeField] private List<ACard> _initialCardsMotherNature;
    [SerializeField] private List<ACard> _ivyCards;
    [SerializeField] private List<ACard> _wildfireCards;
    [SerializeField] private List<ACard> _fragranceCards;

    [SerializeField] private TutorialDialogue[] _initialDialogues;

    //juegan hasta ygdra
    [SerializeField] private TutorialDialogue[] _motherNatureDialogues;

    //el jugador juega ecosistema y poblacion y luego full ronda y luego poner construccion
    [SerializeField] private TutorialDialogue[] _ivyDialogues;

    //el jugador juega hiedra y luego poblacion y luego full ronda
    [SerializeField] private TutorialDialogue[] _wildfireDialogues;

    //el jugador jeuga wildfire luego poblacion luego ronda
    [FormerlySerializedAs("_fragrangeDialogues")] [SerializeField]
    private TutorialDialogue[] _fragranceDialogues;

    //juega el jugador fragancia luego poblacion
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

        var initialCards = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[1]),
        });

        elements.AddRange(_initialDialogues);


        elements.Add(initialCards);
        elements.Add(DrawFixed(_initialCardsMotherNature));


        //turnos sagitario y fung

        var sagitarioTurn1 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(herbivore, sagitarioTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(herbivore, sagitarioTerritory),
        });

        elements.Add(sagitarioTurn1);

        var fungalothTurn1 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(carnivore, fungalothTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, fungalothTerritory),
        });

        elements.Add(fungalothTurn1);


        elements.AddRange(_motherNatureDialogues);

        var forcedMotherNAction = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Ygdra, _initialCardsMotherNature[0], null, 1)
        };
        var playMotherN =
            new TutorialAction(true, null, forcedMotherNAction, true);

        elements.Add(playMotherN);
        elements.Add(new TutorialAction(true));


        var overlordTurn1 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, overlordTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, fungalothTerritory),
        });

        elements.Add(overlordTurn1);

        elements.Add(EcosystemAct());

        elements.Add(DrawFixed(_ivyCards));

        var wipeForIvy = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.RemoveCardsFromTerritoryTutorial(sagitarioTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(fungalothTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(ygdraTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(overlordTerritory),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, overlordTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, overlordTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, overlordTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, sagitarioTerritory.Slots[2]),
            new EffectCommands.PlaceConstructionTutorial(overlordTerritory)
        });
        elements.Add(wipeForIvy);

        elements.AddRange(_ivyDialogues);

        var forcedIvyAction = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Ygdra, _ivyCards[0], null, 1)
        };
        var playivy =
            new TutorialAction(true, null, forcedIvyAction, true);

        elements.Add(playivy);
        elements.Add(new TutorialAction(true));

        //overlord juega

        var overlordTurn2 = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(herbivore, overlordTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(plant, overlordTerritory),
        });

        elements.Add(overlordTurn2);

        elements.Add(EcosystemAct());

        var ivyEffect =
            new TutorialAction(false, ServiceLocator.Get<IRulesSystem>().GetRoundEndObserversEffects);
        elements.Add(ivyEffect);

        elements.Add(DrawFixed(_wildfireCards));
        
        
        var wipeForWildfire = new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.RemoveCardsFromTerritoryTutorial(sagitarioTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(fungalothTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(ygdraTerritory),
            new EffectCommands.RemoveCardsFromTerritoryTutorial(overlordTerritory),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, overlordTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, overlordTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, sagitarioTerritory.Slots[2]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, sagitarioTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, fungalothTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, fungalothTerritory.Slots[2]),
        });
        elements.Add(wipeForWildfire);

        elements.AddRange(_wildfireDialogues);

        var forcedWildfireAction = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Ygdra, _wildfireCards[0], null, 1)
        };
        var playWildfire =
            new TutorialAction(true, null, forcedWildfireAction, true);

        elements.Add(playWildfire);
        elements.Add(new TutorialAction(true));

        
        
        
        elements.Add(DrawFixed(_fragranceCards));
        elements.AddRange(_fragranceDialogues);
        
        
        var forcedFraganceAction = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Ygdra, _fragranceCards[0], null, 1)
        };
        var playFragance =
            new TutorialAction(true, null, forcedFraganceAction, true);

        elements.Add(playFragance);
        elements.Add(new TutorialAction(true));
        
        elements.AddRange(_outroDialogue);


        return elements;
    }

    public override void OnTutorialFinished()
    {
        SceneTransition.Instance.TransitionToCurrentScene();
    }
}