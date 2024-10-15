using System;

public interface IRulebookEntry
{
    public string GetName();
    public string GetDescription();

    public event Action OnDiscard;
}
