public class Wildfire : AInfluenceCard
{
    protected override bool CheckInfluenceCardAction(PlayerAction action)
    {
        if (action.Receivers.Length != 1)
        {
            return false;
        }

        var receiver = action.Receivers[0];

        if (receiver.Location != ValidDropLocation.AnyTerritory)
        {
            return false;
        }

        return true;
    }
}