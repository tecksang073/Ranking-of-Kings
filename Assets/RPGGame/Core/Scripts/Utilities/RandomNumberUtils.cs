public static class RandomNumberUtils
{
    public static float RandomFloat(int seed, float min, float max)
    {
        return (float)(new System.Random(seed).NextDouble() * (max - min) + min);
    }

    public static int RandomInt(int seed, int min, int max)
    {
        return new System.Random(seed).Next(min, max);
    }
}
