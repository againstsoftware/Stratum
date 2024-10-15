using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInteraction : MonoBehaviour
{
    [SerializeField] private GameObject _hostButton, _clientButton, _codeInput;
    [SerializeField] private TextMeshProUGUI _hostCodeText, _playerCountText;
    private string _clientCode;
    private LobbyManager _lobbyManager;
    private LobbyNetwork _lobbyNetwork;

    private void Awake()
    {
        _lobbyManager = GetComponent<LobbyManager>();
        _lobbyNetwork = GetComponent<LobbyNetwork>();
        _lobbyNetwork.OnPlayerCountChange += UpdatePlayerCount;
    }

    private void OnDisable()
    {
        if (_lobbyNetwork is not null) _lobbyNetwork.OnPlayerCountChange -= UpdatePlayerCount;
    }

    public void HostButton()
    {
        _lobbyManager.StartHostAsync(OnHostStartedLocal);
    }

    public void ClientButton()
    {
        _lobbyManager.StartClientAsync(_clientCode, OnClientStartedLocal);
    }

    public void OnEnterCode(string code)
    {
        _clientCode = code;
    }

    public void UpdatePlayerCount(int count)
    {
        _playerCountText.text = $"Connected Players: {count}";
    }

    private void OnHostStartedLocal(string joinCode)
    {
        _hostButton.SetActive(false);
        _clientButton.SetActive(false);
        _codeInput.SetActive(false);
        _hostCodeText.text = $"Join Code: {joinCode}";
    }

    private void OnClientStartedLocal()
    {
        _hostButton.SetActive(false);
        _clientButton.SetActive(false);
        _codeInput.SetActive(false);
    }
}
