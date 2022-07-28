using UnityEngine;

public class ClanBossStage : BaseClanBossStage
{
    [Header("Environment")]
    public EnvironmentData environment;
    [Header("Battle")]
    public CharacterItem character;
    public int level;

    public override PlayerItem GetCharacter()
    {
        return PlayerItem.CreateActorItemWithLevel(character, level);
    }
}
