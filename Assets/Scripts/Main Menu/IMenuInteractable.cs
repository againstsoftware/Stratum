
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMenuInteractable : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    InteractablesObjects InteractableObject {get; }

    void Interact();
}

public enum InteractablesObjects { None, Radio, Book, Registry, Rulebook}
