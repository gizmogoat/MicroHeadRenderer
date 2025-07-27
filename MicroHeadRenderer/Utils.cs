namespace MicroHeadRenderer;

public static class Utils
{
    public static bool IsUUID(string input)
    {
        return Guid.TryParse(input, out Guid _);
    }
}