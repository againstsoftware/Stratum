using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        if(password == _password) Debug.Log("password correcta");
    }
}
