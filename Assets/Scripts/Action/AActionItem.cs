
using System.Collections.Generic;
using UnityEngine;

public abstract class AActionItem : ScriptableObject
{

    public abstract IEnumerable<ValidAction> GetValidActions();
}
