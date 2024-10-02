using UnityEngine;

public interface IInteractable
{
    public GameObject GameObject { get; }
    public PlayerCharacter Owner { get; }
    public bool IsDraggable { get; }
    public ITurnAction Action { get; }

    
    public void OnSelect(); //pasas el raton por encima o tocas una vez en movil
    public void OnDeselect(); //quitas el raton o tocas otro interactable en movil
    public void OnDrag(); //haces click y sostienes o tocas y sostienes en movil
    public void OnDragCancel(); //sueltas el objeto en un sitio no valido o se cancela el drag por lo que sea
    public void OnDrop(IDropLocation dropLocation); //sueltas el objeto en un sitio valido
}
