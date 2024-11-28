
using UnityEngine;

public class RandomizeYapSpeed : MonoBehaviour
{
    public void RandomizeSpeed()
    {
        GetComponentInParent<TutorialRulebook>().RandomizeSpeed();
    }
}
