using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Tutorials/Test Tutorial")]
public class TestTutorial : ATutorialSequence
{
    public override PlayerCharacter LocalPlayer { get; protected set; } = PlayerCharacter.Ygdra;

    [SerializeField] private TutorialDialogue _test;
    public override IEnumerable<ITutorialElement> GetTutorialElements()
    {
        return new[] { _test };
    }

    public override void OnTutorialFinished()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

