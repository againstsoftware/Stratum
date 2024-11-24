
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

    [SerializeField] private float _delayBetweenElements;

    private Queue<ITutorialElement> _tutorialElements;
    private Rulebook _rulebook;
    private ATutorialSequence _tutorialSequence;

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
        Invoke(nameof(ExecuteNextTutorialElement), _delayBetweenElements);
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
            }
        }

        
    }

    private void ExecuteNextTutorialElement()
    {
        if (!_tutorialElements.Any())
        {
            Debug.Log("Tutorial terminade");
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
                PlayerOnTurn = _tutorialSequence.LocalPlayer;
                ServiceLocator.Get<IRulesSystem>().SetForcedAction(action.ForcedActions, action.ForceOnlyActionItem);
            }
            else
            {
                PlayerOnTurn = PlayerCharacter.None;
                ServiceLocator.Get<IRulesSystem>().DisableForcedAction();
                ServiceLocator.Get<IExecutor>().ExecuteRulesEffects(action.GetEffectCommands());
            }
        }
        // OnActionEnded?.Invoke(PlayerOnTurn);
        OnTurnChanged?.Invoke(PlayerOnTurn);
    }

    private void ShowTutorialDialogue(TutorialDialogue dialogue)
    {
        if (_rulebook is null)
            _rulebook = ServiceLocator.Get<IView>().GetViewPlayer(_tutorialSequence.LocalPlayer)
                .GetComponentInChildren<Rulebook>();
        
        _rulebook.DisplayDialogue(dialogue, EndAction);
    }

    
    
    
    public void ChangeTurn(PlayerCharacter playerOnTurn) {}
    public void SyncRNGs() {}
    public void SendTurnChange(PlayerCharacter playerOnTurn) {}
}
