using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInfo : MonoBehaviour
{
    private string _username, _password;

    [SerializeField] private Canvas canvas;

    public void ReceiveUsername(string username)
    {
        _username = username;

        Transform[] canvasChildren = canvas.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in canvasChildren)
        {
            child.gameObject.SetActive(true);
        }

        TextMeshProUGUI usernameText = canvas.transform.Find("Username_TXT").GetComponent<TextMeshProUGUI>();    
        usernameText.text = $"Usuario: {username}";
    }

    public void ReceivePassword(string password)
    {
        _password = password;
    }

    public void CheckPassword(string password)
    {
        if (password != _password) return;
        
        Debug.Log("password correcta");
        
        var txt = canvas.transform.Find("Username_TXT").gameObject;
        
        Transform[] canvasChildren = canvas.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in canvasChildren)
        {
            child.gameObject.SetActive(false);
        }

        txt.SetActive(true);
        txt.GetComponent<TextMeshProUGUI>().text = "Contraseña correcta! (se recargará la escena)";
        
        Invoke(nameof(Reload), 3f);
    }

    private void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
