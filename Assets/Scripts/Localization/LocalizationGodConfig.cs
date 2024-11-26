using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LocalizationGod Config")]
public class LocalizationGodConfig : ScriptableObject
{
    [field: SerializeField] public bool Spanish {get; private set;}
    [field: SerializeField] public List<string> TableNames {get; private set;}

}