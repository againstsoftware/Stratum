
using System.Collections.Generic;
using UnityEngine;

public abstract class AActionItem : ScriptableObject
{

    public abstract IEnumerable<ValidAction> GetValidActions();
    
    public abstract IEnumerable<Effect> GetEffects(int index);
    
    public abstract bool CheckAction(PlayerAction action);

}
