[System.Serializable]
public class FakePlayer
{
    public string profileName;
    public int level;
    public int arenaScore;
    public CharacterItem mainCharacter;
    public int mainCharacterLevel;
    public FakePlayerCharacter[] arenaCharacters;

    public string Id
    {
        get
        {
            return "Fake_" + profileName;
        }
    }

    public int GetExp()
    {
        var exp = 0;
        var gameDb = GameInstance.GameDatabase;
        for (var i = 0; i < level - 1; ++i)
        {
            exp += gameDb.playerExpTable.Calculate(i + 1, gameDb.playerMaxLevel);
        }
        return exp;
    }

    public int GetMainCharacterExp()
    {
        if (mainCharacter == null)
            return 0;
        var exp = 0;
        var itemTier = mainCharacter.itemTier;
        for (var i = 0; i < mainCharacterLevel - 1; ++i)
        {
            exp += itemTier.expTable.Calculate(i + 1, itemTier.maxLevel);
        }
        return exp;
    }

    public Player MakePlayer()
    {
        var entry = new Player();
        entry.Id = Id;
        entry.ProfileName = profileName;
        entry.Exp = GetExp();
        entry.MainCharacter = mainCharacter.Id;
        entry.MainCharacterExp = GetMainCharacterExp();
        entry.ArenaScore = arenaScore;
        return entry;
    }

    [System.Serializable]
    public struct FakePlayerCharacter
    {
        public CharacterItem character;
        public int level;

        public int GetCharacterExp()
        {
            if (character == null)
                return 0;
            var exp = 0;
            var itemTier = character.itemTier;
            for (var i = 0; i < level - 1; ++i)
            {
                exp += itemTier.expTable.Calculate(i + 1, itemTier.maxLevel);
            }
            return exp;
        }

        public PlayerItem MakeAsItem()
        {
            var playerItem = new PlayerItem();
            playerItem.Id = "";
            playerItem.PlayerId = "";
            playerItem.DataId = character.Id;
            playerItem.Amount = 1;
            playerItem.Exp = GetCharacterExp();
            return playerItem;
        }
    }
}
