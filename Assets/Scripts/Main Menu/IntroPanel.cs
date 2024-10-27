using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private string[] _introTexts = new[]
    {
        "¡Bienvenid@ a la versión Alpha de <b>Stratum</b>!",
        "Esta es la version <b>muy</b> primitiva del menú principal.",
        "Interactúa con los objetos haciendo click.",
        "Hay 2 objetos. El primero, 'Pulsa aquí para jugar una partida casual con tus amigos', te llevará a " +
        "la pantalla del Lobby. Ahí podrás crear una sala o unirte a una.",
        "Al crear una sala, te aparecerá un código que debes compartir con tus amigos para que se puedan unir. " +
        "Cuando haya 4 jugadores conectados, comenzará la partida.",
        "Para unirte a una sala, escribe el código del creador de la sala en el campo de texto, y pulsa en 'Unirse'.",
        "El segundo objeto, 'Pulsa aquí para iniciar sesión', te llevará a una página web donde tendrás que iniciar sesión. " +
        "(No te preocupes, puedes poner el usuario y contraseña que quieras, es solo para mostrar la funcionalidad.)",
        "Te aparecerá un cuadro de diálogo para volver al juego, y allí, tendrás que introducir la misma contraseña.",
        "Con esto, si tuvieras una cuenta <b>Premium</b>, podrías jugar el modo competitivo con el sistema de emparejamiento. " +
        "Esto constituye el esqueleto del Modelo de Negocio de <b>Stratum</b>.",
        "Para aprender a jugar, todavía no hay tutorial, pero puedes consultar el reglamento del juego en " +
        "https://github.com/againstsoftware/Stratum",
        "¡Disfruta del juego!"
    };

    private int _idx = 0;

    private void Start()
    {
        _text.text = _introTexts[_idx];
    }

    public void ContinueButton()
    {
        _idx++;
        if (_idx >= _introTexts.Length)
        {
            Destroy(gameObject);
        }
        else
        {
            _text.text = _introTexts[_idx];
        }
    }
}