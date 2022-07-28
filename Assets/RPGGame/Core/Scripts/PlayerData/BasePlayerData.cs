public abstract class BasePlayerData
{
    public static GameDatabase GameDatabase
    {
        get { return GameInstance.GameDatabase; }
    }

    public static BaseGameplayRule GameplayRule
    {
        get { return GameInstance.GameplayRule; }
    }
}
