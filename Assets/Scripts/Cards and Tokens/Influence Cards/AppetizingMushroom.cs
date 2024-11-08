using System;

public class AppetizingMushroom : AInfluenceCard
{
    protected override AInfluenceCardRulesComponent GetRulesComponent() => new AppetizingMushroomRC();

}