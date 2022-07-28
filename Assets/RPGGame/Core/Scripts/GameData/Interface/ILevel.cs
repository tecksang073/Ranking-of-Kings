public interface ILevel
{
    /// <summary>
    /// Current Level
    /// </summary>
    int Level { get; }
    /// <summary>
    /// Max Level
    /// </summary>
    int MaxLevel { get; }
    /// <summary>
    /// Example: 15/20
    /// Current Exp is 15
    /// Required Exp is 20
    /// </summary>
    int CurrentExp { get; }
    /// <summary>
    /// Example: 15/20
    /// Current Exp is 15
    /// Required Exp is 20
    /// </summary>
    int RequiredExp { get; }
}

public static class LevelExpExtention
{
    public static float ExpRate(this ILevel level)
    {
        return (float)level.CurrentExp / (float)level.RequiredExp;
    }

    public static int RequireExp(this ILevel level)
    {
        var requireExp = level.RequiredExp - level.CurrentExp;
        if (requireExp < 0)
            return 0;
        return requireExp;
    }
}