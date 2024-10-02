using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APlayer : MonoBehaviour
{
    
    public bool OnTurn { get; private set; }
    
    
    // private List<Interactable>
}

public enum PlayerCharacter
{
    Ygdra, Sagitario, Fungaloth, Overlord
}
