
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMenuInteractable : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    void EnableInteraction();
    void DisableInteraction();
}

public enum InteractablesObjects { None, Radio, Gramophone, Registry, Rulebook}
