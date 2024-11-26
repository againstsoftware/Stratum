using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using System;
using UnityEngine.Serialization;

public abstract class ATutorialSequence : ScriptableObject
{
    public abstract PlayerCharacter LocalPlayer { get; protected set; }
    
    public abstract IEnumerable<ITutorialElement> GetTutorialElements();

    public abstract void OnTutorialFinished();
    
    protected TutorialAction DrawFixed(List<ACard> cards)
    {
        return new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.DrawFixedCardsTutorial(cards)
        });
    }

    protected TutorialAction EcosystemAct()
    {
        return new TutorialAction(false, new IEffectCommand[]
        {
            new EffectCommands.OverviewSwitch(),
            new EffectCommands.RushEcosystemTurn(),
        });
    }

}

public interface ITutorialElement
{
}

[Serializable]
public class TutorialDialogue : ITutorialElement
{
    // public string Text => _text.GetLocalizedString();
    public string Text => LocalizationGod.GetLocalized("Tutorial", _dialogueText);

    [SerializeField] private LocalizedString _text;

    [FormerlySerializedAs("_newText")] [SerializeField] private string _dialogueText;
}

public class TutorialAction : ITutorialElement
{
    public readonly bool IsPlayerAction;
    public readonly IReadOnlyList<PlayerAction> ForcedActions;
    public readonly bool ForceOnlyActionItem;

    private readonly IEnumerable<IEffectCommand> _effectCommands;
    private Func<IEnumerable<IEffectCommand>> _effectCommandsFunc;
    public TutorialAction(bool isPlayerAction, IEnumerable<IEffectCommand> effectCommands = null, 
        IReadOnlyList<PlayerAction> forcedActions = null, bool forceOnlyActionItem = false)
    {
        IsPlayerAction = isPlayerAction;
        _effectCommands = effectCommands;
        ForcedActions = forcedActions;
        ForceOnlyActionItem = forceOnlyActionItem;
    }

    public TutorialAction(bool isPlayerAction, Func<IEnumerable<IEffectCommand>> effectCommandsFunc)
    {
        _effectCommands = null;
        _effectCommandsFunc = effectCommandsFunc;
    }

    public IEnumerable<IEffectCommand> GetEffectCommands()
    {
        if (_effectCommands is null && _effectCommandsFunc is not null)
            return _effectCommandsFunc();
        else return _effectCommands;
    }
}