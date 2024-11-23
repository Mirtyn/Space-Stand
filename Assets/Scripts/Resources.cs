
public class ResourceHolder
{
    public int Amount;
    public ResourceType Resource;

    public ResourceHolder(int amount, ResourceType type)
    {
        this.Amount = amount;
        this.Resource = type;
    }
}

public enum ResourceType
{
    Coal,
    Wood,
    Iron,
    Uranium,
    Mithril,
    Rubber,
    Copper,
    Gold,
}
