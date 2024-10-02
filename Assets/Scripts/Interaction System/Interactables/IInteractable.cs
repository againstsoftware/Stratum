using UnityEngine;

public interface IInteractable
{
    public PlayerCharacter Owner { get; }


    
    public void OnSelect(); //pasas el raton por encima o tocas una vez en movil
    public void OnDeselect(); //quitas el raton o tocas otro interactable en movil

}
