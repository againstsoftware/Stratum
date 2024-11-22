using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PruebaPasarAGame : MonoBehaviour
{
    public void PasarAGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
