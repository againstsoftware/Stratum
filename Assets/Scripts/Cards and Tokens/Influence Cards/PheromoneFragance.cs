public class PheromoneFragance : AInfluenceCard
{
    protected override AInfluenceCardRulesComponent GetRulesComponent() => new PheromoneFraganceRC();
}