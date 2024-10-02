using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public IInteractable SelectedItem { get; private set; }

    private void Awake()
    {
        if (Instance is null) Instance = this;
        else Destroy(gameObject);
    }

    public void Select(IInteractable item)
    {
        var old = SelectedItem;
        if(old is not null) old.OnDeselect();
        SelectedItem = item;
        item.OnSelect();
    }

    public void Deselect(IInteractable item)
    {
        if (SelectedItem != item)
        {
            Debug.LogError("deselect called with unselected item!");
            return;
        }
        
        if(SelectedItem is not null) SelectedItem.OnDeselect();
        SelectedItem = null;
    }

    public void Drag(IInteractable item)
    {
        
    }
}
