using UnityEngine;


[CreateAssetMenu(menuName = "Game Config")]    
public class GameConfig : ScriptableObject
{
    [field:SerializeField] public int ActionsPerTurn { get; private set; }
    
    [field:SerializeField] public PlayerCharacter[] TurnOrder { get; private set; }
}
