
using UnityEngine;
public class LoginObject: MonoBehaviour, IMenuInteractable
{
    public void OnPointerPress()
    {
        Application.OpenURL("https://oscaralri.github.io/");
    }
    
}
