
using UnityEngine;
public class LoginObject: MonoBehaviour, IMenuInteractable
{
    public void OnPointerPress()
    {
        Application.OpenURL("https://againstsoftware.github.io/Stratum/login/");
    }
    
}
