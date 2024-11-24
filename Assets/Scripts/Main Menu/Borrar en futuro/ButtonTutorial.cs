using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTutorial : MonoBehaviour
{
    public void GoTutorial()
    {
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }
}
