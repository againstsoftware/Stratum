
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Tutorials/Ygdra Tutorial")]

public class YgdraTutorial : ATutorialSequence
{
    public override PlayerCharacter LocalPlayer { get; protected set; } = PlayerCharacter.Ygdra;
    
    [SerializeField] private List<ACard> _initialCardsMotherNature;
    [SerializeField] private List<ACard> _ivyCards;
    [SerializeField] private List<ACard> _wildfireCards;
    [SerializeField] private List<ACard> _fragranceCards;
    
    [SerializeField] private TutorialDialogue[] _initialDialogues;
    //el jugador juega ecosistema y poblacion y luego full ronda y luego poner construccion
    [SerializeField] private TutorialDialogue[] _ivyDialogues;
    //el jugador juega hiedra y luego poblacion y luego full ronda
    [SerializeField] private TutorialDialogue[] _wildfireDialogues;
    //el jugador jeuga wildfire luego poblacion luego ronda
    [SerializeField] private TutorialDialogue[] _fragrangeDialogues;
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
        
        
        
        elements.AddRange(_initialDialogues);
        
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
            
        });
        elements.Add(wipeForIvy);

        return elements;
    }

    public override void OnTutorialFinished()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
