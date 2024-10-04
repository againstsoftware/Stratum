using UnityEngine;

// [CreateAssetMenu(menuName = "Cards/Population Card")]
public class PopulationCard : ACard
{
    public enum Type { Plant, Herbivore, Carnivore }
    [field:SerializeField] public Type PopulationType { get; private set; }
}
