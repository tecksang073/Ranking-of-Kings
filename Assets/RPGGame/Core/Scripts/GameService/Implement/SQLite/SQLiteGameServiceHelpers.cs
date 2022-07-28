using System.Collections.Generic;
using Mono.Data.Sqlite;
using Newtonsoft.Json;

public partial class SQLiteGameService
{
    private bool IsPlayerWithUsernameFound(string type, string username)
    {
        var count = ExecuteScalar(@"SELECT COUNT(*) FROM playerAuth WHERE type=@type AND username=@username",
            new SqliteParameter("@type", type),
            new SqliteParameter("@username", username));
        return count != null && (long)count > 0;
    }

    private Player SetNewPlayerData(Player player)
    {
        player.LoginToken = System.Guid.NewGuid().ToString();
        player.Exp = 0;

        var gameDb = GameInstance.GameDatabase;

        string stageFormationName = string.Empty;
        string arenaFormationName = string.Empty;
        foreach (var formation in gameDb.formations)
        {
            if (formation.formationType == EFormationType.Stage &&
                string.IsNullOrEmpty(stageFormationName))
            {
                stageFormationName = formation.id;
                player.SelectedFormation = stageFormationName;
            }
            if (formation.formationType == EFormationType.Arena &&
                string.IsNullOrEmpty(arenaFormationName))
            {
                arenaFormationName = formation.id;
                player.SelectedArenaFormation = arenaFormationName;
            }
        }

        foreach (var currency in GameInstance.GameDatabase.Currencies.Values)
        {
            var playerCurrency = GetCurrency(player.Id, currency.id);
            playerCurrency.Amount = currency.startAmount + playerCurrency.PurchasedAmount;
            ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                new SqliteParameter("@amount", playerCurrency.Amount),
                new SqliteParameter("@id", playerCurrency.Id));
        }

        ExecuteNonQuery(@"DELETE FROM playerClearStage WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerFormation WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerItem WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerStamina WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerUnlockItem WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));

        for (var i = 0; i < gameDb.startItems.Length; ++i)
        {
            var startItem = gameDb.startItems[i];
            if (startItem == null || startItem.item == null)
                continue;
            var createItems = new List<PlayerItem>();
            var updateItems = new List<PlayerItem>();
            if (AddItems(player.Id, startItem.Id, startItem.amount, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    QueryCreatePlayerItem(createEntry);
                    HelperUnlockItem(player.Id, startItem.Id);
                }
                foreach (var updateEntry in updateItems)
                {
                    QueryUpdatePlayerItem(updateEntry);
                }
            }
        }
        for (var i = 0; i < gameDb.startCharacters.Length; ++i)
        {
            var startCharacter = gameDb.startCharacters[i];
            if (startCharacter == null)
                continue;
            var createItems = new List<PlayerItem>();
            var updateItems = new List<PlayerItem>();
            if (AddItems(player.Id, startCharacter.Id, 1, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    QueryCreatePlayerItem(createEntry);
                    HelperUnlockItem(player.Id, startCharacter.Id);
                    HelperSetFormation(player.Id, createEntry.Id, stageFormationName, i);
                    HelperSetFormation(player.Id, createEntry.Id, arenaFormationName, i);
                }
                foreach (var updateEntry in updateItems)
                {
                    QueryUpdatePlayerItem(updateEntry);
                }
            }
        }
        ExecuteNonQuery(@"UPDATE player SET profileName=@profileName, loginToken=@loginToken, exp=@exp, selectedFormation=@selectedFormation, selectedArenaFormation=@selectedArenaFormation WHERE id=@id",
            new SqliteParameter("@profileName", player.ProfileName),
            new SqliteParameter("@loginToken", player.LoginToken),
            new SqliteParameter("@exp", player.Exp),
            new SqliteParameter("@selectedFormation", player.SelectedFormation),
            new SqliteParameter("@selectedArenaFormation", player.SelectedArenaFormation),
            new SqliteParameter("@id", player.Id));
        return player;
    }

    private Player InsertNewPlayer(string type, string username, string password)
    {
        var playerId = System.Guid.NewGuid().ToString();
        var playerAuth = new PlayerAuth();
        playerAuth.Id = PlayerAuth.GetId(playerId, type);
        playerAuth.PlayerId = playerId;
        playerAuth.Type = type;
        playerAuth.Username = username;
        playerAuth.Password = password;
        ExecuteNonQuery(@"INSERT INTO playerAuth (id, playerId, type, username, password) VALUES (@id, @playerId, @type, @username, @password)",
            new SqliteParameter("@id", playerAuth.Id),
            new SqliteParameter("@playerId", playerAuth.PlayerId),
            new SqliteParameter("@type", playerAuth.Type),
            new SqliteParameter("@username", playerAuth.Username),
            new SqliteParameter("@password", playerAuth.Password));
        var player = new Player();
        player.Id = playerId;
        player = SetNewPlayerData(player);
        UpdatePlayerStamina(player);
        ExecuteNonQuery(@"INSERT INTO player (id, profileName, loginToken, exp, selectedFormation, selectedArenaFormation) VALUES (@id, @profileName, @loginToken, @exp, @selectedFormation, @selectedArenaFormation)",
            new SqliteParameter("@id", player.Id),
            new SqliteParameter("@profileName", player.ProfileName),
            new SqliteParameter("@loginToken", player.LoginToken),
            new SqliteParameter("@exp", player.Exp),
            new SqliteParameter("@selectedFormation", player.SelectedFormation),
            new SqliteParameter("@selectedArenaFormation", player.SelectedArenaFormation));
        return player;
    }

    private bool TryGetPlayer(string type, string username, string password, out Player player)
    {
        player = null;
        var playerAuths = ExecuteReader(@"SELECT * FROM playerAuth WHERE type=@type AND username=@username AND password=@password",
            new SqliteParameter("@type", type),
            new SqliteParameter("@username", username),
            new SqliteParameter("@password", password));
        if (!playerAuths.Read())
            return false;
        var playerAuth = new PlayerAuth();
        playerAuth.Id = playerAuths.GetString("id");
        playerAuth.PlayerId = playerAuths.GetString("playerId");
        playerAuth.Type = playerAuths.GetString("type");
        playerAuth.Username = playerAuths.GetString("username");
        playerAuth.Password = playerAuths.GetString("password");
        var players = ExecuteReader(@"SELECT * FROM player WHERE id=@id",
            new SqliteParameter("@id", playerAuth.PlayerId));
        if (players.Read())
        {
            player = new Player();
            player.Id = players.GetString("id");
            player.ProfileName = players.GetString("profileName");
            player.LoginToken = players.GetString("loginToken");
            player.Exp = players.GetInt32("exp");
            player.SelectedFormation = players.GetString("selectedFormation");
            player.SelectedArenaFormation = players.GetString("selectedArenaFormation");
            player.ArenaScore = players.GetInt32("arenaScore");
            player.HighestArenaRank = players.GetInt32("highestArenaRank");
            player.HighestArenaRankCurrentSeason = players.GetInt32("highestArenaRankCurrentSeason");
        }
        if (player == null)
            return false;
        return true;
    }

    private Player UpdatePlayerLoginToken(Player player)
    {
        player.LoginToken = System.Guid.NewGuid().ToString();
        ExecuteNonQuery(@"UPDATE player SET loginToken=@loginToken WHERE id=@id",
            new SqliteParameter("@loginToken", player.loginToken),
            new SqliteParameter("@id", player.Id));
        return player;
    }

    private bool DecreasePlayerStamina(Player player, Stamina staminaTable, int decreaseAmount)
    {
        var gameDb = GameInstance.GameDatabase;
        var stamina = GetStamina(player.Id, staminaTable.id);
        var maxStamina = staminaTable.maxAmountTable.Calculate(player.Level, gameDb.playerMaxLevel);
        if (stamina.Amount >= decreaseAmount)
        {
            if (stamina.Amount == maxStamina && stamina.Amount - decreaseAmount < maxStamina)
                stamina.RecoveredTime = Timestamp;
            stamina.Amount -= decreaseAmount;
            ExecuteNonQuery(@"UPDATE playerStamina SET amount=@amount, recoveredTime=@recoveredTime WHERE id=@id",
                new SqliteParameter("@amount", stamina.Amount),
                new SqliteParameter("@recoveredTime", stamina.RecoveredTime),
                new SqliteParameter("@id", stamina.Id));
            UpdatePlayerStamina(player, staminaTable);
            return true;
        }
        return false;
    }

    private void UpdatePlayerStamina(Player player, Stamina staminaTable)
    {
        var gameDb = GameInstance.GameDatabase;
        var stamina = GetStamina(player.Id, staminaTable.id);
        var maxStamina = staminaTable.maxAmountTable.Calculate(player.Level, gameDb.playerMaxLevel);
        if (stamina.Amount < maxStamina)
        {
            var currentTimeInSeconds = Timestamp;
            var diffTimeInSeconds = currentTimeInSeconds - stamina.RecoveredTime;
            var devideAmount = 1;
            switch (staminaTable.recoverUnit)
            {
                case StaminaUnit.Days:
                    devideAmount = 60 * 60 * 24;
                    break;
                case StaminaUnit.Hours:
                    devideAmount = 60 * 60;
                    break;
                case StaminaUnit.Minutes:
                    devideAmount = 60;
                    break;
                case StaminaUnit.Seconds:
                    devideAmount = 1;
                    break;
            }
            var recoveryAmount = (int)(diffTimeInSeconds / devideAmount) / staminaTable.recoverDuration;
            if (recoveryAmount > 0 && stamina.Amount < maxStamina)
            {
                stamina.Amount += recoveryAmount;
                if (stamina.Amount > maxStamina)
                    stamina.Amount = maxStamina;
                stamina.RecoveredTime = currentTimeInSeconds;
                ExecuteNonQuery(@"UPDATE playerStamina SET amount=@amount, recoveredTime=@recoveredTime WHERE id=@id",
                    new SqliteParameter("@amount", stamina.Amount),
                    new SqliteParameter("@recoveredTime", stamina.RecoveredTime),
                    new SqliteParameter("@id", stamina.Id));
            }
        }
    }

    private void UpdatePlayerStamina(Player player)
    {
        foreach (var stamina in GameInstance.GameDatabase.Staminas.Values)
        {
            UpdatePlayerStamina(player, stamina);
        }
    }

    private PlayerCurrency GetCurrency(string playerId, string dataId)
    {
        var currencies = ExecuteReader(@"SELECT * FROM playerCurrency WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        PlayerCurrency currency = null;
        if (!currencies.Read())
        {
            currency = new PlayerCurrency();
            currency.Id = PlayerCurrency.GetId(playerId, dataId);
            currency.PlayerId = playerId;
            currency.DataId = dataId;
            ExecuteNonQuery(@"INSERT INTO playerCurrency (id, playerId, dataId, amount, purchasedAmount) VALUES (@id, @playerId, @dataId, @amount, @purchasedAmount)",
                new SqliteParameter("@id", currency.Id),
                new SqliteParameter("@playerId", currency.PlayerId),
                new SqliteParameter("@dataId", currency.DataId),
                new SqliteParameter("@amount", currency.Amount),
                new SqliteParameter("@purchasedAmount", currency.PurchasedAmount));
        }
        else
        {
            currency = new PlayerCurrency();
            currency.Id = currencies.GetString("id");
            currency.PlayerId = currencies.GetString("playerId");
            currency.DataId = currencies.GetString("dataId");
            currency.Amount = currencies.GetInt32("amount");
            currency.PurchasedAmount = currencies.GetInt32("purchasedAmount");
        }
        return currency;
    }

    private PlayerStamina GetStamina(string playerId, string dataId)
    {
        var staminas = ExecuteReader(@"SELECT * FROM playerStamina WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        PlayerStamina stamina = null;
        if (!staminas.Read())
        {
            stamina = new PlayerStamina();
            stamina.Id = PlayerStamina.GetId(playerId, dataId);
            stamina.PlayerId = playerId;
            stamina.DataId = dataId;
            ExecuteNonQuery(@"INSERT INTO playerStamina (id, playerId, dataId, amount, recoveredTime) VALUES (@id, @playerId, @dataId, @amount, @recoveredTime)",
                new SqliteParameter("@id", stamina.Id),
                new SqliteParameter("@playerId", stamina.PlayerId),
                new SqliteParameter("@dataId", stamina.DataId),
                new SqliteParameter("@amount", stamina.Amount),
                new SqliteParameter("@recoveredTime", stamina.RecoveredTime));
        }
        else
        {
            stamina = new PlayerStamina();
            stamina.Id = staminas.GetString("id");
            stamina.PlayerId = staminas.GetString("playerId");
            stamina.DataId = staminas.GetString("dataId");
            stamina.Amount = staminas.GetInt32("amount");
            stamina.RecoveredTime = staminas.GetInt64("recoveredTime");
            stamina.LastRefillTime = staminas.GetInt64("lastRefillTime");
            stamina.RefillCount = staminas.GetInt32("refillCount");
        }
        return stamina;
    }

    private bool AddItems(string playerId,
        string dataId,
        int amount,
        out List<PlayerItem> createItems,
        out List<PlayerItem> updateItems)
    {
        createItems = new List<PlayerItem>();
        updateItems = new List<PlayerItem>();
        BaseItem itemData = null;
        if (!GameInstance.GameDatabase.Items.TryGetValue(dataId, out itemData))
            return false;
        var maxStack = itemData.MaxStack;
        var oldEntries = ExecuteReader(@"SELECT * FROM playerItem WHERE playerId=@playerId AND dataId=@dataId AND amount<@amount",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId),
            new SqliteParameter("@amount", maxStack));
        while (oldEntries.Read())
        {
            var entry = new PlayerItem();
            entry.Id = oldEntries.GetString("id");
            entry.PlayerId = oldEntries.GetString("playerId");
            entry.DataId = oldEntries.GetString("dataId");
            entry.Amount = oldEntries.GetInt32("amount");
            entry.Exp = oldEntries.GetInt32("exp");
            entry.EquipItemId = oldEntries.GetString("equipItemId");
            entry.EquipPosition = oldEntries.GetString("equipPosition");
            try
            {
                entry.RandomedAttributes = JsonConvert.DeserializeObject<CalculatedAttributes>(oldEntries.GetString("randomedAttributes"));
            }
            catch { }
            var sumAmount = entry.Amount + amount;
            if (sumAmount > maxStack)
            {
                entry.Amount = maxStack;
                amount = sumAmount - maxStack;
            }
            else
            {
                entry.Amount += amount;
                amount = 0;
            }
            updateItems.Add(entry);

            if (amount == 0)
                break;
        }
        while (amount > 0)
        {
            var newEntry = new PlayerItem();
            newEntry.PlayerId = playerId;
            newEntry.DataId = dataId;
            if (itemData is BaseActorItem)
                newEntry.RandomedAttributes = (itemData as BaseActorItem).randomAttributes.GetCalculatedAttributes();
            if (amount > maxStack)
            {
                newEntry.Amount = maxStack;
                amount -= maxStack;
            }
            else
            {
                newEntry.Amount = amount;
                amount = 0;
            }
            createItems.Add(newEntry);
        }
        return true;
    }

    private bool UseItems(string playerId,
        string dataId,
        int amount,
        out List<PlayerItem> updateItem,
        out List<string> deleteItemIds,
        bool conditionCanLevelUp = false,
        bool conditionCanEvolve = false,
        bool conditionCanSell = false,
        bool conditionCanBeMaterial = false,
        bool conditionCanBeEquipped = false)
    {
        updateItem = new List<PlayerItem>();
        deleteItemIds = new List<string>();
        if (!GameInstance.GameDatabase.Items.ContainsKey(dataId))
            return false;
        var materials = ExecuteReader(@"SELECT * FROM playerItem WHERE playerId=@playerId AND dataId=@dataId",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        while (materials.Read())
        {
            var material = new PlayerItem();
            material.Id = materials.GetString("id");
            material.PlayerId = materials.GetString("playerId");
            material.DataId = materials.GetString("dataId");
            material.Amount = materials.GetInt32("amount");
            material.Exp = materials.GetInt32("exp");
            material.EquipItemId = materials.GetString("equipItemId");
            material.EquipPosition = materials.GetString("equipPosition");
            try
            {
                material.RandomedAttributes = JsonConvert.DeserializeObject<CalculatedAttributes>(materials.GetString("randomedAttributes"));
            }
            catch { }

            if ((!conditionCanLevelUp || material.CanLevelUp) &&
                (!conditionCanEvolve || material.CanEvolve) &&
                (!conditionCanSell || material.CanSell) &&
                (!conditionCanBeMaterial || material.CanBeMaterial) &&
                (!conditionCanBeEquipped || material.CanBeEquipped))
            {
                if (material.Amount >= amount)
                {
                    material.Amount -= amount;
                    amount = 0;
                }
                else
                {
                    amount -= material.Amount;
                    material.Amount = 0;
                }

                if (material.Amount > 0)
                    updateItem.Add(material);
                else
                    deleteItemIds.Add(material.Id);

                if (amount == 0)
                    break;
            }
        }
        if (amount > 0)
            return false;
        return true;
    }

    private void HelperSetFormation(string playerId, string characterId, string formationName, int position)
    {
        PlayerFormation oldFormation = null;
        if (!string.IsNullOrEmpty(characterId))
        {
            var oldFormations = ExecuteReader(@"SELECT * FROM playerFormation WHERE playerId=@playerId AND dataId=@dataId AND itemId=@itemId",
                new SqliteParameter("@playerId", playerId),
                new SqliteParameter("@dataId", formationName),
                new SqliteParameter("@itemId", characterId));
            while (oldFormations.Read())
            {
                oldFormation = new PlayerFormation();
                oldFormation.Id = oldFormations.GetString("id");
                oldFormation.PlayerId = oldFormations.GetString("playerId");
                oldFormation.DataId = oldFormations.GetString("dataId");
                oldFormation.Position = oldFormations.GetInt32("position");
                oldFormation.ItemId = oldFormations.GetString("itemId");
            }
            if (oldFormation != null)
            {
                ExecuteNonQuery(@"UPDATE playerFormation SET itemId=@itemId WHERE id=@id",
                    new SqliteParameter("@itemId", ""),
                    new SqliteParameter("@id", oldFormation.Id));
            }
        }
        PlayerFormation formation = null;
        var targetFormations = ExecuteReader(@"SELECT * FROM playerFormation WHERE playerId=@playerId AND dataId=@dataId AND position=@position LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", formationName),
            new SqliteParameter("@position", position));
        if (targetFormations.Read())
        {
            formation = new PlayerFormation();
            formation.Id = targetFormations.GetString("id");
            formation.PlayerId = targetFormations.GetString("playerId");
            formation.DataId = targetFormations.GetString("dataId");
            formation.Position = targetFormations.GetInt32("position");
            formation.ItemId = targetFormations.GetString("itemId");
        }
        if (formation == null)
        {
            formation = new PlayerFormation();
            formation.Id = PlayerFormation.GetId(playerId, formationName, position);
            formation.PlayerId = playerId;
            formation.DataId = formationName;
            formation.Position = position;
            formation.ItemId = characterId;
            ExecuteNonQuery(@"INSERT INTO playerFormation (id, playerId, dataId, position, itemId)
                VALUES (@id, @playerId, @dataId, @position, @itemId)",
                new SqliteParameter("@id", formation.Id),
                new SqliteParameter("@playerId", formation.PlayerId),
                new SqliteParameter("@dataId", formation.DataId),
                new SqliteParameter("@position", formation.Position),
                new SqliteParameter("@itemId", formation.ItemId));
        }
        else
        {
            if (oldFormation != null)
            {
                oldFormation.ItemId = formation.ItemId;
                ExecuteNonQuery(@"UPDATE playerFormation SET itemId=@itemId WHERE id=@id",
                    new SqliteParameter("@itemId", oldFormation.ItemId),
                    new SqliteParameter("@id", oldFormation.Id));
            }
            formation.ItemId = characterId;
            ExecuteNonQuery(@"UPDATE playerFormation SET itemId=@itemId WHERE id=@id",
                new SqliteParameter("@itemId", formation.ItemId),
                new SqliteParameter("@id", formation.Id));
        }
    }

    private void HelperUnlockItem(string playerId, string dataId)
    {
        PlayerUnlockItem unlockItem = null;
        var oldUnlockItems = ExecuteReader(@"SELECT * FROM playerUnlockItem WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        if (!oldUnlockItems.Read())
        {
            unlockItem = new PlayerUnlockItem();
            unlockItem.Id = PlayerUnlockItem.GetId(playerId, dataId);
            unlockItem.PlayerId = playerId;
            unlockItem.DataId = dataId;
            unlockItem.Amount = 0;
            ExecuteNonQuery(@"INSERT INTO playerUnlockItem (id, playerId, dataId, amount)
                VALUES (@id, @playerId, @dataId, @amount)",
                new SqliteParameter("@id", unlockItem.Id),
                new SqliteParameter("@playerId", unlockItem.PlayerId),
                new SqliteParameter("@dataId", unlockItem.DataId),
                new SqliteParameter("@amount", unlockItem.Amount));
        }
    }
    private bool HaveEnoughMaterials(string playerId, Dictionary<string, int> selectedMaterials, Dictionary<string, int> requiredMaterials, out List<PlayerItem> updateItems, out List<string> deleteItemIds)
    {
        updateItems = new List<PlayerItem>();
        deleteItemIds = new List<string>();
        var enoughMaterials = true;
        var materialItemIds = selectedMaterials.Keys;
        var materialItems = new List<PlayerItem>();
        foreach (var materialItemId in materialItemIds)
        {
            var foundMaterial = GetPlayerItemById(materialItemId);
            if (foundMaterial == null || foundMaterial.PlayerId != playerId)
                continue;
            materialItems.Add(foundMaterial);
        }
        foreach (var requiredMaterial in requiredMaterials)
        {
            var dataId = requiredMaterial.Key;
            var amount = requiredMaterial.Value;
            foreach (var materialItem in materialItems)
            {
                if (materialItem.DataId != dataId)
                    continue;
                var usingAmount = selectedMaterials[materialItem.Id];
                if (usingAmount > materialItem.Amount)
                    usingAmount = materialItem.Amount;
                if (usingAmount > amount)
                    usingAmount = amount;
                materialItem.Amount -= usingAmount;
                amount -= usingAmount;
                if (materialItem.Amount > 0)
                    updateItems.Add(materialItem);
                else
                    deleteItemIds.Add(materialItem.Id);
                if (amount == 0)
                    break;
            }
            if (amount > 0)
            {
                enoughMaterials = false;
                break;
            }
        }
        return enoughMaterials;
    }

    private bool IsStageAvailable(BaseStage stage)
    {
        var available = true;
        var currentTime = System.DateTime.Now;
        if (stage.availabilities != null && stage.availabilities.Length > 0)
        {
            available = false;
            foreach (var availability in stage.availabilities)
            {
                System.DateTime fromTime = currentTime.Date
                    .AddHours(availability.startTimeHour)
                    .AddMinutes(availability.startTimeMinute);
                System.DateTime toTime = fromTime
                    .AddHours(availability.durationHour)
                    .AddMinutes(availability.durationMinute);
                if (currentTime.DayOfWeek == availability.day &&
                    currentTime.Ticks >= fromTime.Ticks &&
                    currentTime.Ticks < toTime.Ticks)
                {
                    available = true;
                    break;
                }
            }
        }
        if (available)
        {
            if (!stage.hasAvailableDate)
                return true;
            var currentDate = currentTime.Date;
            var startDate = new System.DateTime(stage.startYear, (int)stage.startMonth, stage.startDay);
            var endDate = startDate.AddDays(stage.durationDays);
            return currentDate.Ticks >= startDate.Ticks && currentDate.Ticks < endDate.Ticks;
        }
        return false;
    }

    private FinishStageResult HelperClearStage(FinishStageResult result, Player player, BaseStage stage, int grade)
    {
        var gameDb = GameInstance.GameDatabase;
        PlayerClearStage clearStage = null;
        var clearStages = ExecuteReader(@"SELECT * FROM playerClearStage WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", player.Id),
            new SqliteParameter("@dataId", stage.Id));
        if (!clearStages.Read())
        {
            clearStage = new PlayerClearStage();
            clearStage.Id = PlayerClearStage.GetId(player.Id, stage.Id);
            clearStage.PlayerId = player.Id;
            clearStage.DataId = stage.Id;
            clearStage.BestRating = grade;
            ExecuteNonQuery(@"INSERT INTO playerClearStage (id, playerId, dataId, bestRating)
                VALUES (@id, @playerId, @dataId, @bestRating)",
                new SqliteParameter("@id", clearStage.Id),
                new SqliteParameter("@playerId", clearStage.PlayerId),
                new SqliteParameter("@dataId", clearStage.DataId),
                new SqliteParameter("@bestRating", clearStage.BestRating));
            // First clear rewards
            result.isFirstClear = true;
            result.updateCurrencies.Clear();
            // Player exp
            result.firstClearRewardPlayerExp = stage.firstClearRewardPlayerExp;
            player.Exp += stage.firstClearRewardPlayerExp;
            ExecuteNonQuery(@"UPDATE player SET exp=@exp WHERE id=@playerId",
                new SqliteParameter("@exp", player.Exp),
                new SqliteParameter("@playerId", player.Id));
            result.player = player;
            // Soft currency
            var softCurrency = GetCurrency(player.Id, gameDb.softCurrency.id);
            result.firstClearRewardSoftCurrency = stage.firstClearRewardSoftCurrency;
            softCurrency.Amount += stage.firstClearRewardSoftCurrency;
            ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                new SqliteParameter("@amount", softCurrency.Amount),
                new SqliteParameter("@id", softCurrency.Id));
            result.updateCurrencies.Add(softCurrency);
            // Hard currency
            var hardCurrency = GetCurrency(player.Id, gameDb.hardCurrency.id);
            result.firstClearRewardHardCurrency = stage.firstClearRewardHardCurrency;
            hardCurrency.Amount += stage.firstClearRewardHardCurrency;
            ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                new SqliteParameter("@amount", hardCurrency.Amount),
                new SqliteParameter("@id", hardCurrency.Id));
            result.updateCurrencies.Add(hardCurrency);
            // Items
            for (var i = 0; i < stage.firstClearRewardItems.Length; ++i)
            {
                var rewardItem = stage.firstClearRewardItems[i];
                if (rewardItem == null || rewardItem.item == null)
                    continue;
                var createItems = new List<PlayerItem>();
                var updateItems = new List<PlayerItem>();
                if (AddItems(player.Id, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                {
                    result.firstClearRewardItems.Add(new PlayerItem()
                    {
                        PlayerId = player.Id,
                        DataId = rewardItem.Id,
                        Amount = rewardItem.amount
                    });
                    foreach (var createEntry in createItems)
                    {
                        QueryCreatePlayerItem(createEntry);
                        result.createItems.Add(createEntry);
                        HelperUnlockItem(player.Id, rewardItem.Id);
                    }
                    foreach (var updateEntry in updateItems)
                    {
                        QueryUpdatePlayerItem(updateEntry);
                        result.updateItems.Add(updateEntry);
                    }
                }
                // End add item condition
            }
        }
        else
        {
            clearStage = new PlayerClearStage();
            clearStage.Id = clearStages.GetString("id");
            clearStage.PlayerId = clearStages.GetString("playerId");
            clearStage.DataId = clearStages.GetString("dataId");
            clearStage.BestRating = clearStages.GetInt32("bestRating");
            if (clearStage.BestRating < grade)
            {
                clearStage.BestRating = grade;
                ExecuteNonQuery(@"UPDATE playerClearStage SET bestRating=@bestRating WHERE id=@id",
                    new SqliteParameter("@bestRating", clearStage.BestRating),
                    new SqliteParameter("@id", clearStage.Id));
            }
        }
        result.clearStage = clearStage;
        // Update achievement
        var playerAchievements = GetPlayerAchievements(player.Id);
        var playerClearStages = GetPlayerClearStages(player.Id);
        List<PlayerAchievement> createAchievements;
        List<PlayerAchievement> updateAchievements;
        OfflineAchievementHelpers.UpdateTotalClearStage(player.Id, playerAchievements, playerClearStages, out createAchievements, out updateAchievements);
        foreach (var createEntry in createAchievements)
        {
            QueryCreatePlayerAchievement(createEntry);
        }
        foreach (var updateEntry in updateAchievements)
        {
            QueryUpdatePlayerAchievement(updateEntry);
        }
        OfflineAchievementHelpers.UpdateTotalClearStageRating(player.Id, playerAchievements, playerClearStages, out createAchievements, out updateAchievements);
        foreach (var createEntry in createAchievements)
        {
            QueryCreatePlayerAchievement(createEntry);
        }
        foreach (var updateEntry in updateAchievements)
        {
            QueryUpdatePlayerAchievement(updateEntry);
        }
        OfflineAchievementHelpers.UpdateCountWinStage(player.Id, playerAchievements, out createAchievements, out updateAchievements);
        foreach (var createEntry in createAchievements)
        {
            QueryCreatePlayerAchievement(createEntry);
        }
        foreach (var updateEntry in updateAchievements)
        {
            QueryUpdatePlayerAchievement(updateEntry);
        }
        return result;
    }

    private Player GetPlayerByLoginToken(string playerId, string loginToken)
    {
        Player player = null;
        var players = ExecuteReader(@"SELECT * FROM player WHERE id=@playerId AND loginToken=@loginToken",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@loginToken", loginToken));
        if (players.Read())
        {
            player = new Player();
            player.Id = players.GetString("id");
            player.ProfileName = players.GetString("profileName");
            player.LoginToken = players.GetString("loginToken");
            player.Exp = players.GetInt32("exp");
            player.SelectedFormation = players.GetString("selectedFormation");
            player.SelectedArenaFormation = players.GetString("selectedArenaFormation");
            player.ArenaScore = players.GetInt32("arenaScore");
            player.HighestArenaRank = players.GetInt32("highestArenaRank");
            player.HighestArenaRankCurrentSeason = players.GetInt32("highestArenaRankCurrentSeason");
        }
        return player;
    }

    private PlayerBattle GetPlayerBattleBySession(string playerId, string session)
    {
        PlayerBattle playerBattle = null;
        var playerBattles = ExecuteReader(@"SELECT * FROM playerBattle WHERE playerId=@playerId AND session=@session",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@session", session));
        if (playerBattles.Read())
        {
            playerBattle = new PlayerBattle();
            playerBattle.Id = playerBattles.GetString("id");
            playerBattle.PlayerId = playerBattles.GetString("playerId");
            playerBattle.DataId = playerBattles.GetString("dataId");
            playerBattle.Session = playerBattles.GetString("session");
            playerBattle.BattleResult = (byte)playerBattles.GetInt32("battleResult");
            playerBattle.Rating = playerBattles.GetInt32("rating");
        }
        return playerBattle;
    }

    private PlayerItem GetPlayerItemById(string id)
    {
        PlayerItem playerItem = null;
        var playerItems = ExecuteReader(@"SELECT * FROM playerItem WHERE id=@id",
            new SqliteParameter("@id", id));
        if (playerItems.Read())
        {
            playerItem = new PlayerItem();
            playerItem.Id = playerItems.GetString("id");
            playerItem.PlayerId = playerItems.GetString("playerId");
            playerItem.DataId = playerItems.GetString("dataId");
            playerItem.Amount = playerItems.GetInt32("amount");
            playerItem.Exp = playerItems.GetInt32("exp");
            playerItem.EquipItemId = playerItems.GetString("equipItemId");
            playerItem.EquipPosition = playerItems.GetString("equipPosition");
            try
            {
                playerItem.RandomedAttributes = JsonConvert.DeserializeObject<CalculatedAttributes>(playerItems.GetString("randomedAttributes"));
            }
            catch { }
        }
        return playerItem;
    }

    private PlayerItem GetPlayerItemByEquipper(string playerId, string equipItemId, string equipPosition)
    {
        PlayerItem playerItem = null;
        var playerItems = ExecuteReader(@"SELECT * FROM playerItem WHERE playerId=@playerId AND equipItemId=@equipItemId AND equipPosition=@equipPosition",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@equipItemId", equipItemId),
            new SqliteParameter("@equipPosition", equipPosition));
        if (playerItems.Read())
        {
            playerItem = new PlayerItem();
            playerItem.Id = playerItems.GetString("id");
            playerItem.PlayerId = playerItems.GetString("playerId");
            playerItem.DataId = playerItems.GetString("dataId");
            playerItem.Amount = playerItems.GetInt32("amount");
            playerItem.Exp = playerItems.GetInt32("exp");
            playerItem.EquipItemId = playerItems.GetString("equipItemId");
            playerItem.EquipPosition = playerItems.GetString("equipPosition");
            try
            {
                playerItem.RandomedAttributes = JsonConvert.DeserializeObject<CalculatedAttributes>(playerItems.GetString("randomedAttributes"));
            }
            catch { }
        }
        return playerItem;
    }
}
