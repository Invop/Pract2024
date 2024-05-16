public class CodeGenerator
{
    private static readonly Random Random = new();

    public static int GenerateCode()
    {
        return Random.Next(100000, 999999);
    }
}