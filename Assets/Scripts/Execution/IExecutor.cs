using System;
using System.Collections.Generic;

public interface IExecutor : IService
{
    public bool IsOnTutorial { get; set; }
    public void ExecutePlayerActionEffects(PlayerAction action);
    public void ExecuteRulesEffects(IEnumerable<Effect> effects, Action rulesCallback = null);
    public void ExecuteRulesEffects(IEnumerable<IEffectCommand> effects, Action rulesCallback = null);

    public void PushDelayedCommand(IEffectCommand effectCommand);

}
