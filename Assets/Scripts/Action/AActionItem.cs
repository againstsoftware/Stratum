
using System.Collections.Generic;
using UnityEngine;

public abstract class AActionItem : ScriptableObject
{

    public abstract IEnumerable<ValidAction> GetValidActions();
    
    public abstract IRulesComponent RulesComponent { get; }

}
