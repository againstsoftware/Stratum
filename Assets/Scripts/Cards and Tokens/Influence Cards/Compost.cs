public class Compost : AInfluenceCard
{
    protected override AInfluenceCardRulesComponent GetRulesComponent() => new CompostRC();
}