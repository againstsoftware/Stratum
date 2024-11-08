public class Pesticide : AInfluenceCard
{
    protected override AInfluenceCardRulesComponent GetRulesComponent() => new PesticideRC();
}