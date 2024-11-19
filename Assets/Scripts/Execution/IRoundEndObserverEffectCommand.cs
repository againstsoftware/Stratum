
using System.Collections.Generic;

public interface IRoundEndObserverEffectCommand : IEffectCommand
{
    IEnumerable<IEffectCommand> GetRoundEndEffects();
}
