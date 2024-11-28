using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using System;

public abstract class ATutorialSequence : ScriptableObject
{
    public abstract PlayerCharacter LocalPlayer { get; protected set; }
    public abstract IEnumerable<ITutorialElement> GetTutorialElements();

    public abstract void OnTutorialFinished();

}

public interface ITutorialElement
{
}

[Serializable]
public class TutorialDialogue : ITutorialElement
{
    public string Text => _text.GetLocalizedString();

    [SerializeField] private LocalizedString _text;
}

public class TutorialAction : ITutorialElement
{
    public readonly bool IsPlayerAction;
    public readonly IEnumerable<IEffectCommand> EffectCommands;
    public readonly IReadOnlyList<PlayerAction> ForcedActions;

    public TutorialAction(bool isPlayerAction, IEnumerable<IEffectCommand> effectCommands = null, 
        IReadOnlyList<PlayerAction> forcedActions = null)
    {
        IsPlayerAction = isPlayerAction;
        EffectCommands = effectCommands;
        ForcedActions = forcedActions;
    }
}