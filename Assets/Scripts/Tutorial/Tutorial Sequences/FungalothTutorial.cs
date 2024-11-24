
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Tutorials/Fungaloth Tutorial")]
public class FungalothTutorial : ATutorialSequence
{
    public override PlayerCharacter LocalPlayer { get; protected set; } = PlayerCharacter.Fungaloth;

    [SerializeField] private MacrofungiToken _macrofungiToken;
    
    [SerializeField] private List<ACard> _initialCardsRot;
    [SerializeField] private List<ACard> _cardsMold;

    
    
    [SerializeField] private TutorialDialogue[] _initialDialogues;

    [SerializeField] private TutorialDialogue[] _rotAndThenMacrofungiDialogues;
    
    [SerializeField] private TutorialDialogue[] _moldAndThenMacrofungiDialogues;
    
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
            new EffectCommands.PlaceCardOnSlotTutorial(carnivore, overlordTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, sagitarioTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, ygdraTerritory.Slots[3]),
            new EffectCommands.PlaceCardOnSlotTutorial(mushroom, fungalothTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(mushroom, fungalothTerritory.Slots[1]),
        });
        

        elements.AddRange(_initialDialogues);

        elements.Add(initialCards);

        elements.Add(DrawFixed(_initialCardsRot));
        
        elements.AddRange(_rotAndThenMacrofungiDialogues);

        var forceRot = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Fungaloth, _initialCardsRot[0], null, 1)
        };
        var playRot =
            new TutorialAction(true, null, forceRot, true);

        var forceMacrofungi = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Fungaloth, _macrofungiToken, null, 1)
        };
        var playMacrofungi =
            new TutorialAction(true, null, forceMacrofungi, true);

        
        elements.Add(playRot);
        elements.Add(playMacrofungi);
        
        var wipeConstructionThenMold = new TutorialAction(false, new IEffectCommand[]
        {
            
            new EffectCommands.PlaceCardOnFreeSlotTutorial(mushroom, sagitarioTerritory),
            new EffectCommands.PlaceCardOnFreeSlotTutorial(mushroom, sagitarioTerritory),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[0]),
            new EffectCommands.PlaceCardOnSlotTutorial(plant, ygdraTerritory.Slots[1]),
            new EffectCommands.PlaceCardOnSlotTutorial(herbivore, ygdraTerritory.Slots[2]),
            new EffectCommands.PlaceConstructionTutorial(ygdraTerritory)
        });
        
        elements.Add(wipeConstructionThenMold);
        
        elements.Add(DrawFixed(_cardsMold));

        
        elements.AddRange(_moldAndThenMacrofungiDialogues);
        
        var forceMold = new List<PlayerAction>()
        {
            new PlayerAction(PlayerCharacter.Fungaloth, _cardsMold[0], null, 1)
        };
        var playMold =
            new TutorialAction(true, null, forceMold, true);
        
        elements.Add(playMold);
        elements.Add(playMacrofungi);
        
        elements.AddRange(_outroDialogue);


        return elements;
    }

    public override void OnTutorialFinished()
    {
        SceneManager.LoadScene(0);
    }
}
