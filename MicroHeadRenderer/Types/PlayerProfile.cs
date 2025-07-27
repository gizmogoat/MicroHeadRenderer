namespace MicroHeadRenderer.Types;

public class Property
{
    public string name { get; set; }
    public string value { get; set; }
}

public class PlayerProfile
{
    public string id { get; set; }
    public string name { get; set; }
    public List<Property> properties { get; set; }
}
