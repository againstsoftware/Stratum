
using System.Collections.Generic;

public interface IExecutor : IService
{

    public void ExecuteEffectCommands(PlayerAction action);

}
