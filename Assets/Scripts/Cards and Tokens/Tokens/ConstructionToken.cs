
public class ConstructionToken : AToken
{
    public override IRulesComponent RulesComponent => GetRC();

    private IRulesComponent GetRC() => new ConstructionTokenRC();
}