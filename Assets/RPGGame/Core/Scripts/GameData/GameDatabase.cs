using System.Collections.Generic;
using UnityEngine;

public partial class GameDatabase : ScriptableObject
{
    [Header("Player database")]
    [Range(1, 1000)]
    public int playerMaxLevel;
    [Tooltip("Requires Exp to levelup for each level")]
    public Int32Attribute playerExpTable;
    [Tooltip("`Soft Currency`, `Start Amount` is start amount when create new player")]
    public Currency softCurrency = new Currency() { id = "GOLD", startAmount = 0 };
    [Tooltip("`Hard Currency`, `Start Amount` is start amount when create new player")]
    public Currency hardCurrency = new Currency() { id = "GEM", startAmount = 0 };
    public Currency[] customCurrencies;
    [Tooltip("Stamina which will be used to enter stage")]
    public Stamina stageStamina = new Stamina() { id = "STAGE_STAMINA", maxAmountTable = new Int32Attribute() };
    [Tooltip("Stamina which will be used to enter arena")]
    public Stamina arenaStamina = new Stamina() { id = "ARENA_STAMINA", maxAmountTable = new Int32Attribute() };
    public Stamina[] customStaminas;
    public Formation[] formations = new Formation[] {
        new Formation() { id = "STAGE_FORMATION_A", formationType = EFormationType.Stage },
        new Formation() { id = "STAGE_FORMATION_B", formationType = EFormationType.Stage },
        new Formation() { id = "STAGE_FORMATION_C", formationType = EFormationType.Stage },
        new Formation() { id = "ARENA_FORMATION_A", formationType = EFormationType.Arena },
        new Formation() { id = "ARENA_FORMATION_B", formationType = EFormationType.Arena },
        new Formation() { id = "ARENA_FORMATION_C", formationType = EFormationType.Arena },
    };

    [Header("Item Levelup Mechanic")]
    public UIItemListFilterSetting characterLevelUpMaterialFilter = new UIItemListFilterSetting()
    {
        showCharacter = true
    };
    public UIItemListFilterSetting equipmentLevelUpMaterialFilter = new UIItemListFilterSetting()
    {
        showEquipment = true
    };

    [Header("Item database")]
    [Tooltip("List of game items, place all items here (includes character, equipment)")]
    public BaseItem[] items;

    [Header("Stage database")]
    [Tooltip("List of game stages, place all stages here")]
    public BaseStage[] stages;

    [Header("Raid Boss Stage database")]
    [Tooltip("List of raid-boss stages, place all stages here")]
    public BaseRaidBossStage[] raidBossStages;

    [Header("Clan Boss Stage database")]
    [Tooltip("List of clan-boss stages, place all stages here")]
    public BaseClanBossStage[] clanBossStages;

    [Header("Achievement database")]
    [Tooltip("List of achievements, place all achievements here")]
    public Achievement[] achievements;

    [Header("Item Craft Formula database")]
    [Tooltip("List of item craft formula, place all item craft formula here")]
    public ItemCraftFormula[] itemCrafts;

    [Header("Loot Box database")]
    [Tooltip("List of game loot boxes, place all loot boxes here")]
    public LootBox[] lootBoxes;

    [Header("Random Store database")]
    [Tooltip("List of game random store, place all random store here")]
    public RandomStore[] randomStores;

    [Header("In-Game Package database")]
    [Tooltip("List of game In-Game packages, place all In-Game packages here")]
    public InGamePackage[] inGamePackages;

    [Header("In-App Purchasing Package database")]
    [Tooltip("List of game IAP packages, place all IAP packages here")]
    public IapPackage[] iapPackages;

    [Header("Hard Currency Conversion")]
    public int hardToSoftCurrencyConversion;

    [Header("Game beginning")]
    [Tooltip("List of start items, place items that you want to give to players when begin the game")]
    public ItemAmount[] startItems;
    [Tooltip("List of start characters, characters in this list will joined team formation when begin the game")]
    public CharacterItem[] startCharacters;
    [Tooltip("List of stages that will be unlocked when begin the game")]
    public BaseStage[] unlockStages;

    [Header("Fake data")]
    [Tooltip("List of fake players that will be shown in helper selection before start battle scene")]
    public FakePlayer[] fakePlayers;

    [Header("Gameplay")]
    [Tooltip("Base attributes for all characters while battle")]
    public CalculatedAttributes characterBaseAttributes;
    [Tooltip("Price to revive all characters when all characters dies, this use hard currency")]
    public int revivePrice = 5;
    [Tooltip("This will caculate with sum Atk to random Atk as: Atk = Mathf.Random(Atk * minAtkVaryRate, Atk * maxAtkVaryRate)")]
    public float minAtkVaryRate;
    [Tooltip("This will caculate with sum Atk to random Atk as: Atk = Mathf.Random(Atk * minAtkVaryRate, Atk * maxAtkVaryRate)")]
    public float maxAtkVaryRate;
    [Tooltip("If this is true, system will reset item level to 1 when evolved")]
    public bool resetItemLevelAfterEvolve;

    [Header("Arena")]
    public ArenaRank[] arenaRanks;
    public int arenaWinScoreIncrease;
    public int arenaLoseScoreDecrease;
    public BaseEnvironmentData arenaEnvironment;

    [Header("Clan")]
    [Range(1, 1000)]
    public int clanMaxLevel;
    [Tooltip("Requires Exp to levelup for each level")]
    public Int32Attribute clanExpTable;
    public CreateClanRequirementType createClanCurrencyType;
    public int createClanCurrencyAmount;
    public int clanCheckinRewardClanExp;
    public CurrencyAmount[] clanCheckinRewardCurrencies;
    public ClanDonation[] clanDonations;
    public byte maxClanDonation = 5;

    public readonly Dictionary<string, BaseItem> Items = new Dictionary<string, BaseItem>();
    public readonly Dictionary<string, ClanDonation> ClanDonations = new Dictionary<string, ClanDonation>();
    public readonly Dictionary<string, Currency> Currencies = new Dictionary<string, Currency>();
    public readonly Dictionary<string, Stamina> Staminas = new Dictionary<string, Stamina>();
    public readonly Dictionary<string, Formation> Formations = new Dictionary<string, Formation>();
    public readonly Dictionary<string, BaseStage> Stages = new Dictionary<string, BaseStage>();
    public readonly Dictionary<string, BaseRaidBossStage> RaidBossStages = new Dictionary<string, BaseRaidBossStage>();
    public readonly Dictionary<string, BaseClanBossStage> ClanBossStages = new Dictionary<string, BaseClanBossStage>();
    public readonly Dictionary<string, Achievement> Achievements = new Dictionary<string, Achievement>();
    public readonly Dictionary<string, ItemCraftFormula> ItemCrafts = new Dictionary<string, ItemCraftFormula>();
    public readonly Dictionary<string, LootBox> LootBoxes = new Dictionary<string, LootBox>();
    public readonly Dictionary<string, RandomStore> RandomStores = new Dictionary<string, RandomStore>();
    public readonly Dictionary<string, InGamePackage> InGamePackages = new Dictionary<string, InGamePackage>();
    public readonly Dictionary<string, IapPackage> IapPackages = new Dictionary<string, IapPackage>();
    public readonly Dictionary<string, FakePlayer> FakePlayers = new Dictionary<string, FakePlayer>();

    public void Setup()
    {
        Items.Clear();
        ClanDonations.Clear();
        Currencies.Clear();
        Staminas.Clear();
        Formations.Clear();
        Stages.Clear();
        RaidBossStages.Clear();
        ClanBossStages.Clear();
        Achievements.Clear();
        ItemCrafts.Clear();
        LootBoxes.Clear();
        RandomStores.Clear();
        InGamePackages.Clear();
        IapPackages.Clear();
        FakePlayers.Clear();

        AddItemsToDatabase(items);

        var startItemList = new List<BaseItem>();
        if (startItems != null && startItems.Length > 0)
        {
            foreach (var startItem in startItems)
            {
                startItemList.Add(startItem.item);
            }
        }
        AddItemsToDatabase(startItemList);

        var startCharacterList = new List<BaseItem>();
        if (startCharacters != null && startCharacters.Length > 0)
        {
            foreach (var startCharacter in startCharacters)
            {
                startCharacterList.Add(startCharacter);
            }
        }
        AddItemsToDatabase(startCharacterList);

        if (clanDonations != null && clanDonations.Length > 0)
        {
            foreach (var clanDonation in clanDonations)
            {
                ClanDonations[clanDonation.id] = clanDonation;
            }
        }

        Currencies[softCurrency.id] = softCurrency;
        Currencies[hardCurrency.id] = hardCurrency;
        if (customCurrencies != null && customCurrencies.Length > 0)
        {
            foreach (var currency in customCurrencies)
            {
                Currencies[currency.id] = currency;
            }
        }

        Staminas[stageStamina.id] = stageStamina;
        Staminas[arenaStamina.id] = arenaStamina;
        if (customStaminas != null && customStaminas.Length > 0)
        {
            foreach (var stamina in customStaminas)
            {
                Staminas[stamina.id] = stamina;
            }
        }

        AddFormationsToDatabase(formations);
        AddStagesToDatabase(stages);
        AddRaidBossStagesToDatabase(raidBossStages);
        AddClanBossStagesToDatabase(clanBossStages);
        AddStagesToDatabase(unlockStages);
        AddAchievementsToDatabase(achievements);
        AddItemCraftsToDatabase(itemCrafts);
        AddLootBoxesToDatabase(lootBoxes);
        AddRandomStoresToDatabase(randomStores);
        AddInGamePackagesToDatabase(inGamePackages);
        AddIapPackagesToDatabase(iapPackages);
        AddFakePlayersToDatabase(fakePlayers);
    }

    private void AddItemsToDatabase(IEnumerable<BaseItem> items)
    {
        foreach (var item in items)
        {
            if (item == null)
                continue;
            var dataId = item.Id;
            if (!string.IsNullOrEmpty(dataId) && !Items.ContainsKey(dataId))
            {
                Items[dataId] = item;
            }
        }
    }

    private void AddFormationsToDatabase(IEnumerable<Formation> formations)
    {
        foreach (var formation in formations)
        {
            if (formation == null)
                continue;
            var dataId = formation.id;
            if (!string.IsNullOrEmpty(dataId) && !Formations.ContainsKey(dataId))
            {
                Formations[dataId] = formation;
            }
        }
    }

    private void AddStagesToDatabase(IEnumerable<BaseStage> stages)
    {
        foreach (var stage in stages)
        {
            if (stage == null)
                continue;
            var dataId = stage.Id;
            if (!string.IsNullOrEmpty(dataId) && !Stages.ContainsKey(dataId))
            {
                Stages[dataId] = stage;
            }
        }
    }

    private void AddRaidBossStagesToDatabase(IEnumerable<BaseRaidBossStage> raidBossStages)
    {
        foreach (var raidBossStage in raidBossStages)
        {
            if (raidBossStage == null)
                continue;
            var dataId = raidBossStage.Id;
            if (!string.IsNullOrEmpty(dataId) && !RaidBossStages.ContainsKey(dataId))
            {
                RaidBossStages[dataId] = raidBossStage;
            }
        }
    }

    private void AddClanBossStagesToDatabase(IEnumerable<BaseClanBossStage> clanBossStages)
    {
        foreach (var clanBossStage in clanBossStages)
        {
            if (clanBossStage == null)
                continue;
            var dataId = clanBossStage.Id;
            if (!string.IsNullOrEmpty(dataId) && !ClanBossStages.ContainsKey(dataId))
            {
                ClanBossStages[dataId] = clanBossStage;
            }
        }
    }

    private void AddAchievementsToDatabase(IEnumerable<Achievement> achievements)
    {
        foreach (var achievement in achievements)
        {
            if (achievement == null)
                continue;
            var dataId = achievement.Id;
            if (!string.IsNullOrEmpty(dataId) && !Achievements.ContainsKey(dataId))
            {
                Achievements[dataId] = achievement;
            }
        }
    }

    private void AddItemCraftsToDatabase(IEnumerable<ItemCraftFormula> itemCrafts)
    {
        foreach (var itemCraft in itemCrafts)
        {
            if (itemCraft == null)
                continue;
            var dataId = itemCraft.Id;
            if (!string.IsNullOrEmpty(dataId) && !ItemCrafts.ContainsKey(dataId))
            {
                ItemCrafts[dataId] = itemCraft;
            }
        }
    }

    private void AddLootBoxesToDatabase(IEnumerable<LootBox> lootBoxes)
    {
        foreach (var lootBox in lootBoxes)
        {
            if (lootBox == null)
                continue;
            var dataId = lootBox.Id;
            if (!string.IsNullOrEmpty(dataId) && !LootBoxes.ContainsKey(dataId))
            {
                LootBoxes[dataId] = lootBox;
            }
        }
    }

    private void AddRandomStoresToDatabase(IEnumerable<RandomStore> randomStores)
    {
        foreach (var randomStore in randomStores)
        {
            if (randomStore == null)
                continue;
            var dataId = randomStore.Id;
            if (!string.IsNullOrEmpty(dataId) && !RandomStores.ContainsKey(dataId))
            {
                RandomStores[dataId] = randomStore;
            }
        }
    }

    private void AddInGamePackagesToDatabase(IEnumerable<InGamePackage> inGamePackages)
    {
        foreach (var inGamePackage in inGamePackages)
        {
            if (inGamePackage == null)
                continue;
            var dataId = inGamePackage.Id;
            if (!string.IsNullOrEmpty(dataId) && !InGamePackages.ContainsKey(dataId))
            {
                InGamePackages[dataId] = inGamePackage;
            }
        }
    }

    private void AddIapPackagesToDatabase(IEnumerable<IapPackage> iapPackages)
    {
        foreach (var iapPackage in iapPackages)
        {
            if (iapPackage == null)
                continue;
            var dataId = iapPackage.Id;
            if (!string.IsNullOrEmpty(dataId) && !IapPackages.ContainsKey(dataId))
            {
                IapPackages[dataId] = iapPackage;
            }
        }
    }

    private void AddFakePlayersToDatabase(IEnumerable<FakePlayer> fakePlayers)
    {
        foreach (var fakePlayer in fakePlayers)
        {
            if (fakePlayer == null)
                continue;
            var dataId = fakePlayer.Id;
            if (!string.IsNullOrEmpty(dataId) && !FakePlayers.ContainsKey(dataId))
            {
                FakePlayers[dataId] = fakePlayer;
            }
        }
    }

    public string ToJson()
    {
        var gameDatabase = this;
        var achievementsJson = string.Empty;
        var itemCraftsJson = string.Empty;
        var itemsJson = string.Empty;
        var clanDonationsJson = string.Empty;
        var currenciesJson = string.Empty;
        var staminasJson = string.Empty;
        var formationsJson = string.Empty;
        var stagesJson = string.Empty;
        var raidBossStagesJson = string.Empty;
        var clanBossStagesJson = string.Empty;
        var lootBoxesJson = string.Empty;
        var randomStoresJson = string.Empty;
        var iapPackagesJson = string.Empty;
        var inGamePackagesJson = string.Empty;
        var startItemsJson = string.Empty;
        var startCharactersJson = string.Empty;
        var unlockStagesJson = string.Empty;
        var arenaRanksJson = string.Empty;
        var clanCheckinRewardCurrenciesJson = string.Empty;

        foreach (var achievement in gameDatabase.Achievements)
        {
            if (!string.IsNullOrEmpty(achievementsJson))
                achievementsJson += ",";
            achievementsJson += "\"" + achievement.Key + "\":" + achievement.Value.ToJson();
        }
        achievementsJson = "{" + achievementsJson + "}";

        foreach (var itemCraft in gameDatabase.ItemCrafts)
        {
            if (!string.IsNullOrEmpty(itemCraftsJson))
                itemCraftsJson += ",";
            itemCraftsJson += "\"" + itemCraft.Key + "\":" + itemCraft.Value.ToJson();
        }
        itemCraftsJson = "{" + itemCraftsJson + "}";

        foreach (var item in gameDatabase.Items)
        {
            if (!string.IsNullOrEmpty(itemsJson))
                itemsJson += ",";
            itemsJson += "\"" + item.Key + "\":" + item.Value.ToJson();
        }
        itemsJson = "{" + itemsJson + "}";

        foreach (var clanDonation in gameDatabase.ClanDonations)
        {
            if (!string.IsNullOrEmpty(clanDonationsJson))
                clanDonationsJson += ",";
            clanDonationsJson += "\"" + clanDonation.Key + "\":" + clanDonation.Value.ToJson();
        }
        clanDonationsJson = "{" + clanDonationsJson + "}";

        foreach (var currency in gameDatabase.Currencies)
        {
            if (!string.IsNullOrEmpty(currenciesJson))
                currenciesJson += ",";
            currenciesJson += "\"" + currency.Key + "\":" + currency.Value.ToJson();
        }
        currenciesJson = "{" + currenciesJson + "}";

        foreach (var stamina in gameDatabase.Staminas)
        {
            if (!string.IsNullOrEmpty(staminasJson))
                staminasJson += ",";
            staminasJson += "\"" + stamina.Key + "\":" + stamina.Value.ToJson();
        }
        staminasJson = "{" + staminasJson + "}";

        foreach (var entry in gameDatabase.Formations)
        {
            if (!string.IsNullOrEmpty(formationsJson))
                formationsJson += ",";
            formationsJson += "\"" + entry.Key + "\"";
        }
        formationsJson = "[" + formationsJson + "]";

        foreach (var entry in gameDatabase.Stages)
        {
            if (!string.IsNullOrEmpty(stagesJson))
                stagesJson += ",";
            stagesJson += "\"" + entry.Key + "\":" + entry.Value.ToJson();
        }
        stagesJson = "{" + stagesJson + "}";

        foreach (var entry in gameDatabase.RaidBossStages)
        {
            if (!string.IsNullOrEmpty(raidBossStagesJson))
                raidBossStagesJson += ",";
            raidBossStagesJson += "\"" + entry.Key + "\":" + entry.Value.ToJson();
        }
        raidBossStagesJson = "{" + raidBossStagesJson + "}";

        foreach (var entry in gameDatabase.ClanBossStages)
        {
            if (!string.IsNullOrEmpty(clanBossStagesJson))
                clanBossStagesJson += ",";
            clanBossStagesJson += "\"" + entry.Key + "\":" + entry.Value.ToJson();
        }
        clanBossStagesJson = "{" + clanBossStagesJson + "}";

        foreach (var entry in gameDatabase.LootBoxes)
        {
            if (!string.IsNullOrEmpty(lootBoxesJson))
                lootBoxesJson += ",";
            lootBoxesJson += "\"" + entry.Key + "\":" + entry.Value.ToJson();
        }
        lootBoxesJson = "{" + lootBoxesJson + "}";

        foreach (var entry in gameDatabase.RandomStores)
        {
            if (!string.IsNullOrEmpty(randomStoresJson))
                randomStoresJson += ",";
            randomStoresJson += "\"" + entry.Key + "\":" + entry.Value.ToJson();
        }
        randomStoresJson = "{" + randomStoresJson + "}";

        foreach (var entry in gameDatabase.IapPackages)
        {
            if (!string.IsNullOrEmpty(iapPackagesJson))
                iapPackagesJson += ",";
            iapPackagesJson += "\"" + entry.Key + "\":" + entry.Value.ToJson();
        }
        iapPackagesJson = "{" + iapPackagesJson + "}";

        foreach (var entry in gameDatabase.InGamePackages)
        {
            if (!string.IsNullOrEmpty(inGamePackagesJson))
                inGamePackagesJson += ",";
            inGamePackagesJson += "\"" + entry.Key + "\":" + entry.Value.ToJson();
        }
        inGamePackagesJson = "{" + inGamePackagesJson + "}";

        foreach (var entry in gameDatabase.startItems)
        {
            if (entry == null || entry.item == null)
                continue;
            if (!string.IsNullOrEmpty(startItemsJson))
                startItemsJson += ",";
            startItemsJson += entry.ToJson();
        }
        startItemsJson = "[" + startItemsJson + "]";

        foreach (var entry in gameDatabase.startCharacters)
        {
            if (entry == null)
                continue;
            if (!string.IsNullOrEmpty(startCharactersJson))
                startCharactersJson += ",";
            startCharactersJson += "\"" + entry.Id + "\"";
        }
        startCharactersJson = "[" + startCharactersJson + "]";

        foreach (var entry in gameDatabase.unlockStages)
        {
            if (entry == null)
                continue;
            if (!string.IsNullOrEmpty(unlockStagesJson))
                unlockStagesJson += ",";
            unlockStagesJson += "\"" + entry.Id + "\"";
        }
        unlockStagesJson = "[" + unlockStagesJson + "]";

        foreach (var entry in gameDatabase.arenaRanks)
        {
            if (entry == null)
                continue;
            if (!string.IsNullOrEmpty(arenaRanksJson))
                arenaRanksJson += ",";
            arenaRanksJson += entry.ToJson();
        }
        arenaRanksJson = "[" + arenaRanksJson + "]";

        foreach (var entry in gameDatabase.clanCheckinRewardCurrencies)
        {
            if (!string.IsNullOrEmpty(clanCheckinRewardCurrenciesJson))
                clanCheckinRewardCurrenciesJson += ",";
            clanCheckinRewardCurrenciesJson += entry.ToJson();
        }
        clanCheckinRewardCurrenciesJson = "[" + clanCheckinRewardCurrenciesJson + "]";

        Dictionary<string, string> keyValues = new Dictionary<string, string>();
        keyValues["softCurrencyId"] = $"\"{softCurrency.id}\"";
        keyValues["hardCurrencyId"] = $"\"{hardCurrency.id}\"";
        keyValues["stageStaminaId"] = $"\"{stageStamina.id}\"";
        keyValues["arenaStaminaId"] = $"\"{arenaStamina.id}\"";
        keyValues["achievements"] = achievementsJson;
        keyValues["itemCrafts"] = itemCraftsJson;
        keyValues["items"] = itemsJson;
        keyValues["clanDonations"] = clanDonationsJson;
        keyValues["currencies"] = currenciesJson;
        keyValues["staminas"] = staminasJson;
        keyValues["formations"] = formationsJson;
        keyValues["stages"] = stagesJson;
        keyValues["raidBossStages"] = raidBossStagesJson;
        keyValues["clanBossStages"] = clanBossStagesJson;
        keyValues["lootBoxes"] = lootBoxesJson;
        keyValues["randomStores"] = randomStoresJson;
        keyValues["iapPackages"] = iapPackagesJson;
        keyValues["inGamePackages"] = inGamePackagesJson;
        keyValues["hardToSoftCurrencyConversion"] = gameDatabase.hardToSoftCurrencyConversion.ToString();
        keyValues["startItems"] = startItemsJson;
        keyValues["startCharacters"] = startCharactersJson;
        keyValues["unlockStages"] = unlockStagesJson;
        keyValues["arenaRanks"] = arenaRanksJson;
        keyValues["arenaWinScoreIncrease"] = gameDatabase.arenaWinScoreIncrease.ToString();
        keyValues["arenaLoseScoreDecrease"] = gameDatabase.arenaLoseScoreDecrease.ToString();
        keyValues["playerMaxLevel"] = gameDatabase.playerMaxLevel.ToString();
        keyValues["playerExpTable"] = gameDatabase.playerExpTable.ToJson();
        keyValues["clanExpTable"] = gameDatabase.clanExpTable.ToJson();
        keyValues["revivePrice"] = gameDatabase.revivePrice.ToString();
        keyValues["resetItemLevelAfterEvolve"] = (gameDatabase.resetItemLevelAfterEvolve ? 1 : 0).ToString();
        keyValues["createClanCurrencyType"] = ((byte)gameDatabase.createClanCurrencyType).ToString();
        keyValues["createClanCurrencyAmount"] = gameDatabase.createClanCurrencyAmount.ToString();
        keyValues["clanCheckinRewardClanExp"] = gameDatabase.clanCheckinRewardClanExp.ToString();
        keyValues["clanCheckinRewardCurrencies"] = clanCheckinRewardCurrenciesJson;
        keyValues["maxClanDonation"] = maxClanDonation.ToString();

        DevExtUtils.InvokeInstanceDevExtMethods(this, "AddExportingData", keyValues);

        int keyValuesCount = keyValues.Count;
        int count = 0;
        string json = string.Empty;
        foreach (var keyValue in keyValues)
        {
            if (count == 0)
                json += "{";
            if (count > 0)
                json += ",";
            json += $"\"{keyValue.Key}\":{keyValue.Value}";
            if (count == keyValuesCount - 1)
                json += "}";
            count++;
        }
        return json;
    }
}
