
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    private static readonly int _fadeIn = Animator.StringToHash("fade in");
    private static readonly int _fadeOut = Animator.StringToHash("fade out");

    private Animator _animator;

    private string _currentSceneToLoad;
    private bool _loadWithNetwork;
    
    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
        _animator = GetComponent<Animator>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded();
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void TransitionToScene(int buildIndex, bool net = false)
    {
        TransitionToScene(SceneManager.GetSceneByBuildIndex(buildIndex).name, net);
    }
    

    public void TransitionToCurrentScene(bool net = false)
    {
        TransitionToScene(SceneManager.GetActiveScene().buildIndex, net);   
    }
    
    public void TransitionToScene(string sceneName, bool net = false)
    {
        _loadWithNetwork = net;
        _currentSceneToLoad = sceneName;
        _animator.Play(_fadeOut);
    }

    public void OnFadeOutEnd()
    {
        if (_currentSceneToLoad is null) return;
        
        if (_loadWithNetwork)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(_currentSceneToLoad, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(_currentSceneToLoad, LoadSceneMode.Single);
        }

        _currentSceneToLoad = null;
        _loadWithNetwork = false;
    }


    private void OnSceneLoaded(Scene scene = default, LoadSceneMode mode = default)
    {
        _animator.Play(_fadeIn);
    }
}
