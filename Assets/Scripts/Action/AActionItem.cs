
using UnityEngine;

public abstract class AActionItem : ScriptableObject
{
    public abstract ValidAction[] ValidActions { get; protected set; }

}
