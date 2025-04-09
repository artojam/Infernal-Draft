

public class RandomSelector
{
    private static readonly System.Random _random = new System.Random();

    public static RoomConfig SelectRandom(RoomConfig[] items)
    {
        int randomValue = _random.Next(100);
        byte cumulative = 0;

        foreach (var item in items)
        {
            cumulative += item.chance;
            if (cumulative > randomValue)
                return item;
        }

        return items[^1];
    }
}
