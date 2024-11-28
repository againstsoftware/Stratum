using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTutorial : MonoBehaviour
{
    public void GoTutorial()
    {
        SceneTransition.Instance.TransitionToScene("Tutorial");
    }
}
