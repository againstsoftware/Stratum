
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour, ITurnSystem, ICommunicationSystem
{
    public PlayerCharacter PlayerOnTurn { get; private set; }
    public bool IsAuthority => true;
    public bool IsRNGSynced => true;
    
    public event Action<PlayerCharacter> OnTurnChanged;
    public event Action<PlayerCharacter> OnActionEnded;
    
    public event Action OnGameStart;

    [SerializeField] private TutorialRulebook _tutorialRulebook;
    [SerializeField] private float _delayBetweenElements;

    private Queue<ITutorialElement> _tutorialElements;
    private ATutorialSequence _tutorialSequence;
    private bool _isCurrentPlayerAction;

    private void Start()
    {
        _tutorialSequence = TutorialProgression.Instance.GetTutorial();
        _tutorialElements = new(_tutorialSequence.GetTutorialElements());
        
        SetLocalPlayer();

        PlayerOnTurn = PlayerCharacter.None;
    }
    
    public void StartGame()
    {

        OnGameStart?.Invoke();
        
        // ExecuteNextTutorialElement();
    }
    
    public void SendActionToAuthority(PlayerAction action)
    {
        ServiceLocator.Get<IExecutor>().ExecutePlayerActionEffects(action);
    }

    public void EndAction()
    {
        if (_isCurrentPlayerAction)
        {
            _isCurrentPlayerAction = false;
            Invoke(nameof(ExecuteNextTutorialElement), .01f);
        }
        else
        {
            Invoke(nameof(ExecuteNextTutorialElement), _delayBetweenElements);
        }
    }

    private void SetLocalPlayer()
    {
        var localPlayer = _tutorialSequence.LocalPlayer;
        foreach (PlayerCharacter character in Enum.GetValues(typeof(PlayerCharacter)))
        {
            if (character is PlayerCharacter.None) continue;

            var viewPlayer = ServiceLocator.Get<IView>().GetViewPlayer(character);
            var cam = viewPlayer.MainCamera;
            
            if (character != localPlayer)
            {
                Destroy(cam.gameObject);
                Destroy(viewPlayer.UICamera.gameObject);
            }
            else
            {
                viewPlayer.IsLocalPlayer = true;
                
                ServiceLocator.Get<IInteractionSystem>().SetLocalPlayer(localPlayer, cam);
                ServiceLocator.Get<IView>().SetLocalPlayer(localPlayer, cam);
                _tutorialRulebook.SetLocalPlayer(localPlayer, cam);
            }
        }
        
        
    }

    private void ExecuteNextTutorialElement()
    {
        if (!_tutorialElements.Any())
        {
            _tutorialSequence.OnTutorialFinished();
            return;
        }

        var element = _tutorialElements.Dequeue();

        if (element is TutorialDialogue dialogue)
        {
            PlayerOnTurn = PlayerCharacter.None;
            ShowTutorialDialogue(dialogue);
            ServiceLocator.Get<IRulesSystem>().DisableForcedAction();
        }
        else if (element is TutorialAction action)
        {
            if (action.IsPlayerAction)
            {
                _isCurrentPlayerAction = true;
                PlayerOnTurn = _tutorialSequence.LocalPlayer;
                ServiceLocator.Get<IModel>().AdvanceTurn(PlayerOnTurn);
                ServiceLocator.Get<IRulesSystem>().SetForcedAction(action.ForcedActions, action.ForceOnlyActionItem);
            }
            else
            {
                PlayerOnTurn = PlayerCharacter.None;
                ServiceLocator.Get<IRulesSystem>().DisableForcedAction();
                ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(action.GetEffectCommands());
            }
        }
        OnTurnChanged?.Invoke(PlayerOnTurn);
    }

    private void ShowTutorialDialogue(TutorialDialogue dialogue)
    {
        _tutorialRulebook.DisplayTutorialDialogue(dialogue, EndAction);
    }

    
    
    
    public void ChangeTurn(PlayerCharacter playerOnTurn) {}
    public void SyncRNGs() {}
    public void SendTurnChange(PlayerCharacter playerOnTurn) {}
}
