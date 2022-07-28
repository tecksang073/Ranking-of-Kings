using System.Collections.Generic;
using UnityEngine;

public partial class LiteDbGameService
{
    private bool IsPlayerWithUsernameFound(string type, string username)
    {
        var playerAuth = colPlayerAuth.FindOne(a => a.Type == type && a.Username == username);
        return playerAuth != null;
    }

    private DbPlayer SetNewPlayerData(DbPlayer player)
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
            colPlayerCurrency.Update(playerCurrency);
        }

        colPlayerClearStage.Delete(a => a.PlayerId == player.Id);
        colPlayerFormation.Delete(a => a.PlayerId == player.Id);
        colPlayerItem.Delete(a => a.PlayerId == player.Id);
        colPlayerStamina.Delete(a => a.PlayerId == player.Id);
        colPlayerUnlockItem.Delete(a => a.PlayerId == player.Id);

        for (var i = 0; i < gameDb.startItems.Length; ++i)
        {
            var startItem = gameDb.startItems[i];
            if (startItem == null || startItem.item == null)
                continue;
            var createItems = new List<DbPlayerItem>();
            var updateItems = new List<DbPlayerItem>();
            if (AddItems(player.Id, startItem.Id, startItem.amount, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    createEntry.Id = System.Guid.NewGuid().ToString();
                    colPlayerItem.Insert(createEntry);
                    HelperUnlockItem(player.Id, startItem.Id);
                }
                foreach (var updateEntry in updateItems)
                {
                    colPlayerItem.Update(updateEntry);
                }
            }
        }
        for (var i = 0; i < gameDb.startCharacters.Length; ++i)
        {
            var startCharacter = gameDb.startCharacters[i];
            if (startCharacter == null)
                continue;
            var createItems = new List<DbPlayerItem>();
            var updateItems = new List<DbPlayerItem>();
            if (AddItems(player.Id, startCharacter.Id, 1, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    createEntry.Id = System.Guid.NewGuid().ToString();
                    colPlayerItem.Insert(createEntry);
                    HelperUnlockItem(player.Id, startCharacter.Id);
                    HelperSetFormation(player.Id, createEntry.Id, stageFormationName, i);
                    HelperSetFormation(player.Id, createEntry.Id, arenaFormationName, i);
                }
                foreach (var updateEntry in updateItems)
                {
                    colPlayerItem.Update(updateEntry);
                }
            }
        }
        colPlayer.Update(player);
        return player;
    }

    private DbPlayer InsertNewPlayer(string type, string username, string password)
    {
        var playerId = System.Guid.NewGuid().ToString();
        var playerAuth = new DbPlayerAuth();
        playerAuth.Id = PlayerAuth.GetId(playerId, type);
        playerAuth.PlayerId = playerId;
        playerAuth.Type = type;
        playerAuth.Username = username;
        playerAuth.Password = password;
        colPlayerAuth.Insert(playerAuth);
        var player = new DbPlayer();
        player.Id = playerId;
        player = SetNewPlayerData(player);
        UpdatePlayerStamina(player);
        colPlayer.Insert(player);
        return player;
    }

    private bool TryGetPlayer(string type, string username, string password, out DbPlayer player)
    {
        player = null;
        var playerAuth = colPlayerAuth.FindOne(a => a.Type == type && a.Username == username && a.Password == password);
        if (playerAuth == null)
            return false;
        player = colPlayer.FindOne(a => a.Id == playerAuth.PlayerId);
        if (player == null)
            return false;
        return true;
    }

    private DbPlayer UpdatePlayerLoginToken(DbPlayer player)
    {
        player.LoginToken = System.Guid.NewGuid().ToString();
        colPlayer.Update(player);
        return player;
    }

    private bool DecreasePlayerStamina(DbPlayer player, Stamina staminaTable, int decreaseAmount)
    {
        var gameDb = GameInstance.GameDatabase;
        var stamina = GetStamina(player.Id, staminaTable.id);
        var gamePlayer = Player.CloneTo(player, new Player());
        var maxStamina = staminaTable.maxAmountTable.Calculate(gamePlayer.Level, gameDb.playerMaxLevel);
        if (stamina.Amount >= decreaseAmount)
        {
            if (stamina.Amount == maxStamina && stamina.Amount - decreaseAmount < maxStamina)
                stamina.RecoveredTime = Timestamp;
            stamina.Amount -= decreaseAmount;
            colPlayerStamina.Update(stamina);
            UpdatePlayerStamina(player, staminaTable);
            return true;
        }
        return false;
    }

    private void UpdatePlayerStamina(DbPlayer player, Stamina staminaTable)
    {
        var gameDb = GameInstance.GameDatabase;

        var stamina = GetStamina(player.Id, staminaTable.id);
        var gamePlayer = Player.CloneTo(player, new Player());
        var maxStamina = staminaTable.maxAmountTable.Calculate(gamePlayer.Level, gameDb.playerMaxLevel);
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
                colPlayerStamina.Update(stamina);
            }
        }
    }

    private void UpdatePlayerStamina(DbPlayer player)
    {
        foreach (var stamina in GameInstance.GameDatabase.Staminas.Values)
        {
            UpdatePlayerStamina(player, stamina);
        }
    }

    private DbPlayerCurrency GetCurrency(string playerId, string dataId)
    {
        var currency = colPlayerCurrency.FindOne(a => a.PlayerId == playerId && a.DataId == dataId);
        if (currency == null)
        {
            currency = new DbPlayerCurrency();
            currency.Id = PlayerCurrency.GetId(playerId, dataId);
            currency.PlayerId = playerId;
            currency.DataId = dataId;
            colPlayerCurrency.Insert(currency);
        }
        return currency;
    }

    private DbPlayerStamina GetStamina(string playerId, string dataId)
    {
        var stamina = colPlayerStamina.FindOne(a => a.PlayerId == playerId && a.DataId == dataId);
        if (stamina == null)
        {
            stamina = new DbPlayerStamina();
            stamina.Id = PlayerStamina.GetId(playerId, dataId);
            stamina.PlayerId = playerId;
            stamina.DataId = dataId;
            colPlayerStamina.Insert(stamina);
        }
        return stamina;
    }

    private bool AddItems(string playerId,
        string dataId,
        int amount,
        out List<DbPlayerItem> createItems,
        out List<DbPlayerItem> updateItems)
    {
        createItems = new List<DbPlayerItem>();
        updateItems = new List<DbPlayerItem>();
        BaseItem itemData = null;
        if (!GameInstance.GameDatabase.Items.TryGetValue(dataId, out itemData))
            return false;
        var maxStack = itemData.MaxStack;
        var oldEntries = colPlayerItem.Find(a => a.DataId == dataId && a.PlayerId == playerId && a.Amount < maxStack);
        foreach (var entry in oldEntries)
        {
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
            var newEntry = new DbPlayerItem();
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
        out List<DbPlayerItem> updateItem,
        out List<string> deleteItemIds,
        bool conditionCanLevelUp = false,
        bool conditionCanEvolve = false,
        bool conditionCanSell = false,
        bool conditionCanBeMaterial = false,
        bool conditionCanBeEquipped = false)
    {
        updateItem = new List<DbPlayerItem>();
        deleteItemIds = new List<string>();
        if (!GameInstance.GameDatabase.Items.ContainsKey(dataId))
            return false;
        var materials = colPlayerItem.Find(a => a.DataId == dataId && a.PlayerId == playerId);
        foreach (var material in materials)
        {
            var gamePlayerItem = PlayerItem.CloneTo(material, new PlayerItem());

            if ((!conditionCanLevelUp || gamePlayerItem.CanLevelUp) &&
                (!conditionCanEvolve || gamePlayerItem.CanEvolve) &&
                (!conditionCanSell || gamePlayerItem.CanSell) &&
                (!conditionCanBeMaterial || gamePlayerItem.CanBeMaterial) &&
                (!conditionCanBeEquipped || gamePlayerItem.CanBeEquipped))
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
        DbPlayerFormation oldFormation = null;
        if (!string.IsNullOrEmpty(characterId))
        {
            oldFormation = colPlayerFormation.FindOne(a => a.PlayerId == playerId && a.DataId == formationName && a.ItemId == characterId);
            if (oldFormation != null)
            {
                oldFormation.ItemId = string.Empty;
                colPlayerFormation.Update(oldFormation);
            }
        }
        var formation = colPlayerFormation.FindOne(a => a.PlayerId == playerId && a.DataId == formationName && a.Position == position);
        if (formation == null)
        {
            formation = new DbPlayerFormation();
            formation.Id = PlayerFormation.GetId(playerId, formationName, position);
            formation.PlayerId = playerId;
            formation.DataId = formationName;
            formation.Position = position;
            formation.ItemId = characterId;
            colPlayerFormation.Insert(formation);
        }
        else
        {
            if (oldFormation != null)
            {
                oldFormation.ItemId = formation.ItemId;
                colPlayerFormation.Update(oldFormation);
            }
            formation.ItemId = characterId;
            colPlayerFormation.Update(formation);
        }
    }

    private DbPlayerUnlockItem HelperUnlockItem(string playerId, string dataId)
    {
        var unlockItem = colPlayerUnlockItem.FindById(PlayerUnlockItem.GetId(playerId, dataId));
        if (unlockItem == null)
        {
            unlockItem = new DbPlayerUnlockItem();
            unlockItem.Id = PlayerUnlockItem.GetId(playerId, dataId);
            unlockItem.PlayerId = playerId;
            unlockItem.DataId = dataId;
            unlockItem.Amount = 0;
            colPlayerUnlockItem.Insert(unlockItem);
        }
        return unlockItem;
    }

    private FinishStageResult HelperClearStage(FinishStageResult result, DbPlayer player, BaseStage stage, int grade)
    {
        var gameDb = GameInstance.GameDatabase;
        var clearStage = colPlayerClearStage.FindById(PlayerClearStage.GetId(player.Id, stage.Id));
        if (clearStage == null)
        {
            clearStage = new DbPlayerClearStage();
            clearStage.Id = PlayerClearStage.GetId(player.Id, stage.Id);
            clearStage.PlayerId = player.Id;
            clearStage.DataId = stage.Id;
            clearStage.BestRating = grade;
            colPlayerClearStage.Insert(clearStage);
            // First clear rewards
            result.isFirstClear = true;
            result.updateCurrencies.Clear();
            // Player exp
            result.firstClearRewardPlayerExp = stage.firstClearRewardPlayerExp;
            player.Exp += stage.firstClearRewardPlayerExp;
            colPlayer.Update(player);
            result.player = Player.CloneTo(player, new Player());
            // Soft currency
            var softCurrency = GetCurrency(player.Id, gameDb.softCurrency.id);
            result.firstClearRewardSoftCurrency = stage.firstClearRewardSoftCurrency;
            softCurrency.Amount += stage.firstClearRewardSoftCurrency;
            colPlayerCurrency.Update(softCurrency);
            result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
            // Hard currency
            var hardCurrency = GetCurrency(player.Id, gameDb.hardCurrency.id);
            result.firstClearRewardHardCurrency = stage.firstClearRewardHardCurrency;
            hardCurrency.Amount += stage.firstClearRewardHardCurrency;
            colPlayerCurrency.Update(hardCurrency);
            result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
            // Items
            for (var i = 0; i < stage.firstClearRewardItems.Length; ++i)
            {
                var rewardItem = stage.firstClearRewardItems[i];
                if (rewardItem == null || rewardItem.item == null)
                    continue;
                var createItems = new List<DbPlayerItem>();
                var updateItems = new List<DbPlayerItem>();
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
                        createEntry.Id = System.Guid.NewGuid().ToString();
                        colPlayerItem.Insert(createEntry);
                        var resultItem = PlayerItem.CloneTo(createEntry, new PlayerItem());
                        result.createItems.Add(resultItem);
                        HelperUnlockItem(player.Id, rewardItem.Id);
                    }
                    foreach (var updateEntry in updateItems)
                    {
                        colPlayerItem.Update(updateEntry);
                        var resultItem = PlayerItem.CloneTo(updateEntry, new PlayerItem());
                        result.updateItems.Add(resultItem);
                    }
                }
                // End add item condition
            }
        }
        else
        {
            if (clearStage.BestRating < grade)
            {
                clearStage.BestRating = grade;
                colPlayerClearStage.Update(clearStage);
            }
        }
        result.clearStage = PlayerClearStage.CloneTo(clearStage, new PlayerClearStage());
        // Update achievement
        var playerAchievements = DbPlayerAchievement.CloneList(colPlayerAchievement.Find(a => a.PlayerId == player.Id));
        var playerClearStages = DbPlayerClearStage.CloneList(colPlayerClearStage.Find(a => a.PlayerId == player.Id));
        List<PlayerAchievement> createAchievements;
        List<PlayerAchievement> updateAchievements;
        OfflineAchievementHelpers.UpdateTotalClearStage(player.Id, playerAchievements, playerClearStages, out createAchievements, out updateAchievements);
        foreach (var createEntry in createAchievements)
        {
            createEntry.Id = System.Guid.NewGuid().ToString();
            colPlayerAchievement.Insert(PlayerAchievement.CloneTo(createEntry, new DbPlayerAchievement()));
        }
        foreach (var updateEntry in updateAchievements)
        {
            colPlayerAchievement.Update(PlayerAchievement.CloneTo(updateEntry, new DbPlayerAchievement()));
        }
        OfflineAchievementHelpers.UpdateTotalClearStageRating(player.Id, playerAchievements, playerClearStages, out createAchievements, out updateAchievements);
        foreach (var createEntry in createAchievements)
        {
            createEntry.Id = System.Guid.NewGuid().ToString();
            colPlayerAchievement.Insert(PlayerAchievement.CloneTo(createEntry, new DbPlayerAchievement()));
        }
        foreach (var updateEntry in updateAchievements)
        {
            colPlayerAchievement.Update(PlayerAchievement.CloneTo(updateEntry, new DbPlayerAchievement()));
        }
        OfflineAchievementHelpers.UpdateCountWinStage(player.Id, playerAchievements, out createAchievements, out updateAchievements);
        foreach (var createEntry in createAchievements)
        {
            createEntry.Id = System.Guid.NewGuid().ToString();
            colPlayerAchievement.Insert(PlayerAchievement.CloneTo(createEntry, new DbPlayerAchievement()));
        }
        foreach (var updateEntry in updateAchievements)
        {
            colPlayerAchievement.Update(PlayerAchievement.CloneTo(updateEntry, new DbPlayerAchievement()));
        }
        return result;
    }

    private bool HaveEnoughMaterials(string playerId, Dictionary<string, int> selectedMaterials, Dictionary<string, int> requiredMaterials, out List<PlayerItem> updateItems, out List<string> deleteItemIds)
    {
        updateItems = new List<PlayerItem>();
        deleteItemIds = new List<string>();
        var enoughMaterials = true;
        var selectedMaterialIds = selectedMaterials.Keys;
        var materialItems = new List<PlayerItem>();
        foreach (var materialItemId in selectedMaterialIds)
        {
            var foundMaterial = colPlayerItem.FindOne(a => a.Id == materialItemId && a.PlayerId == playerId);
            if (foundMaterial == null)
                continue;
            var resultItem = PlayerItem.CloneTo(foundMaterial, new PlayerItem());
            materialItems.Add(resultItem);
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
                Debug.LogError(requiredMaterial.Key + " " + amount + " vs " + requiredMaterial.Value);
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
}
