using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugUI : MonoBehaviour
{
    private int _herbivores, _carnivores, _plants;
    private bool _gameOver;
    private string _gameOverText;

    private void Start()
    {
        var model = ServiceLocator.Get<IModel>();
        model.OnCardPlaced += UpdateInfo;
        model.OnCardRemoved += UpdateInfo;
        ServiceLocator.Get<IRulesSystem>().OnGameOver += OnGameOver;
    }

    private void OnGUI()
    {
        GUIStyle textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.white;
        textStyle.alignment = TextAnchor.MiddleCenter;
        if (_gameOver)
        {
            textStyle.fontSize = 70;
            GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 50, 500, 100), _gameOverText, textStyle);
            return;
        }


        textStyle.fontSize = 30;

        GUI.Label(new Rect(50, 50, 200, 30), $"Plantas: {_plants}", textStyle);
        GUI.Label(new Rect(50, 100, 200, 30), $"Herviboros: {_herbivores}", textStyle);
        GUI.Label(new Rect(50, 150, 200, 30), $"Carnivoros: {_carnivores}", textStyle);
    }

    private void UpdateInfo()
    {
        _herbivores = ServiceLocator.Get<IModel>().Ecosystem.Herbivores.Count;
        _carnivores = ServiceLocator.Get<IModel>().Ecosystem.Carnivores.Count;
        _plants = ServiceLocator.Get<IModel>().Ecosystem.Plants.Count;
    }

    private void OnGameOver(PlayerCharacter[] winners)
    {
        _gameOver = true;

        _gameOverText = "PARTIDA TERMINADA. GANADOR(ES):\n";
        foreach (var w in winners) _gameOverText += $"{w}\n";
        
        Invoke(nameof(Restart), 2f);
    }

    private void Restart() => SceneManager.LoadScene(0);
}