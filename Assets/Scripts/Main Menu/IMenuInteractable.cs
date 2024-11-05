
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMenuInteractable : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    InteractablesObjects InteractableObject {get; }
    void Interact();
    void EnableInteraction();
    void DisableInteraction();
}

public enum InteractablesObjects { None, Radio, Gramophone, Registry, Rulebook}
