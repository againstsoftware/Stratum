
using System.Collections.Generic;

public interface IRoundEndObserver
{
    IEnumerable<IEffectCommand> GetRoundEndEffects();
}
