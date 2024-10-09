using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Game Config")]    
public class GameConfig : ScriptableObject
{
    [field:SerializeField] public int ActionsPerTurn { get; private set; }
    
    [field:SerializeField] public PlayerCharacter[] TurnOrder { get; private set; }

    [SerializeField] private AActionItem[] _actionItems;

    public int ActionItemToID(AActionItem actionItem) => Array.IndexOf(_actionItems, actionItem);
    public AActionItem IDToActionItem(int id) => _actionItems[id];
}
