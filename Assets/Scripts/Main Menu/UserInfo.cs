using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    private string _username, _password;

    [SerializeField] private Canvas canvas;

    // metodo que se llama desde la web
    public void ReceiveUsername(string username)
    {
        Debug.Log($"username {username}");
        _username = username;

        canvas.gameObject.SetActive(true);
        TextMeshProUGUI usernameText = canvas.transform.Find("Username_TXT").GetComponent<TextMeshProUGUI>();    
        usernameText.text = $"Usuario: {username}";
    }

    public void ReceivePassword(string password)
    {
        Debug.Log($"password {password}");
        _password = password;
    }

    public void CheckPassword(string password)
    {
        if(password == _password) Debug.Log("password correcta");
    }
}
