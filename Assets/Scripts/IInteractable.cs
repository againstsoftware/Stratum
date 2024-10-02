using UnityEngine;

public interface IInteractable
{
    public PlayerCharacter Owner { get; }

    
    public void OnSelect(); //pasas el raton por encima o tocas una vez en movil
    public void OnDeselect(); //quitas el raton o tocas otro interactable en movil
    public void OnDragStart(); //haces click y sostienes o tocas y sostienes en movil
    public void OnDragCancel(); //sueltas el objeto sobre algo no vallido o se cancela el drag por lo que sea
    
    public void OnDragItemHover(IInteractable draggingItem); //estas arrastrando un objeto y lo pones encima del interactable sin soltar
    public void OnDragItemRelease(IInteractable draggingItem); //estas arrastrando un objeto y sueltas encima del interactable

}
