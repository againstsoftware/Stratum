public class MacrofungiToken : AToken
{
    public override IRulesComponent RulesComponent => GetRC();

    private IRulesComponent GetRC() => new MacrofungiTokenRC();
}