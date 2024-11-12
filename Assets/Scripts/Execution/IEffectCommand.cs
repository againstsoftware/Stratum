
using System;

public interface IEffectCommand
{
    public void Execute(PlayerAction action, Action callback);
}
