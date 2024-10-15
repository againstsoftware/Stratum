using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviour
{
    [SerializeField] private int _maxConnections;
    
    public void StartHostAsync(Action<string> callback)
    {
        StartCoroutine(StartHostCoroutine(callback));
    }
    
    public void StartClientAsync(string joinCode, Action callback)
    {
        StartCoroutine(StartClientCoroutine(joinCode, callback));
    }
    
    
    
        
    private IEnumerator StartHostCoroutine(Action<string> callback)
    {
        Task<string> hostTask = StartHostWithRelay();
        while (!hostTask.IsCompleted) yield return null;

        if (hostTask.Status == TaskStatus.RanToCompletion)
        {
            Debug.Log("Host created successfully.");
            callback(hostTask.Result);
        }
        else
        {
            Debug.Log($"Host Task Canceled or Faulted:  {hostTask.Exception}");
        }
    }
    
    private IEnumerator StartClientCoroutine(string joinCode, Action callback)
    {
        Task<bool> clientTask = StartClientWithRelay(joinCode);
        while (!clientTask.IsCompleted) yield return null;

        if (clientTask.Status == TaskStatus.RanToCompletion)
        {
            if (!clientTask.Result) Debug.Log("Client failed.");
            else
            {
                Debug.Log("Client created successfully.");
                callback();
            }

        }
        else
        {
            Debug.Log($"Client Task Canceled or Faulted:  {clientTask.Exception}");
        }
    }
    
    private async Task<string> StartHostWithRelay()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxConnections - 1);
        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetRelayServerData(new RelayServerData(allocation, "wss"));
        unityTransport.UseWebSockets = true;

        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    private async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "wss"));
        unityTransport.UseWebSockets = true;
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
    
    
}