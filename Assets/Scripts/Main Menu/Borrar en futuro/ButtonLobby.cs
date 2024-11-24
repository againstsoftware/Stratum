using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonLobby : MonoBehaviour
{
    public void OnClickGoLobby()
    {
        SceneManager.LoadScene("Lobby Test", LoadSceneMode.Single);
    }
}
