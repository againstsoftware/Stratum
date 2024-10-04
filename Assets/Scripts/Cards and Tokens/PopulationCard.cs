using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Population Card")]
public class PopulationCard : ACard
{
    public enum Type { Plant, Herbivore, Carnivore }

    [SerializeField] private Type _populationType;

    public Type[] GetTypes()
    {
        throw new NotImplementedException();
    }

}
