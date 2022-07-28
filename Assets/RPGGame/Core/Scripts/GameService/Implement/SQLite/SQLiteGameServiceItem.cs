using System.Collections.Generic;
using UnityEngine.Events;
using Mono.Data.Sqlite;

public partial class SQLiteGameService
{
    protected override void DoLevelUpItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = GetPlayerByLoginToken(playerId, loginToken);
        var foundItem = GetPlayerItemById(itemId);
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundItem == null || foundItem.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
            var levelUpPrice = foundItem.LevelUpPrice;
            var requireCurrency = 0;
            var increasingExp = 0;
            var updateItems = new List<PlayerItem>();
            var deleteItemIds = new List<string>();
            var materialItemIds = materials.Keys;
            var materialItems = new List<PlayerItem>();
            foreach (var materialItemId in materialItemIds)
            {
                var foundMaterial = GetPlayerItemById(materialItemId);
                if (foundMaterial == null || foundMaterial.PlayerId != playerId)
                    continue;

                if (foundMaterial.CanBeMaterial)
                    materialItems.Add(foundMaterial);
            }
            foreach (var materialItem in materialItems)
            {
                var usingAmount = materials[materialItem.Id];
                if (usingAmount > materialItem.Amount)
                    usingAmount = materialItem.Amount;
                requireCurrency += levelUpPrice * usingAmount;
                increasingExp += materialItem.RewardExp * usingAmount;
                materialItem.Amount -= usingAmount;
                if (materialItem.Amount > 0)
                    updateItems.Add(materialItem);
                else
                    deleteItemIds.Add(materialItem.Id);
            }
            if (requireCurrency > softCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
            else
            {
                softCurrency.Amount -= requireCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", softCurrency.Amount),
                    new SqliteParameter("@id", softCurrency.Id));
                foundItem = foundItem.CreateLevelUpItem(increasingExp);
                updateItems.Add(foundItem);
                foreach (var updateItem in updateItems)
                {
                    QueryUpdatePlayerItem(updateItem);
                }
                foreach (var deleteItemId in deleteItemIds)
                {
                    ExecuteNonQuery(@"DELETE FROM playerItem WHERE id=@id", new SqliteParameter("@id", deleteItemId));
                }
                result.updateCurrencies.Add(softCurrency);
                result.updateItems = updateItems;
                result.deleteItemIds = deleteItemIds;
                // Update achievement
                if (foundItem.ActorItemData.Type.Equals("CharacterItem") ||
                    foundItem.ActorItemData.Type.Equals("EquipmentItem"))
                {
                    var playerAchievements = GetPlayerAchievements(playerId);
                    List<PlayerAchievement> createAchievements = null;
                    List<PlayerAchievement> updateAchievements = null;
                    if (foundItem.ActorItemData.Type.Equals("CharacterItem"))
                        OfflineAchievementHelpers.UpdateCountLevelUpCharacter(playerId, playerAchievements, out createAchievements, out updateAchievements);
                    if (foundItem.ActorItemData.Type.Equals("EquipmentItem"))
                        OfflineAchievementHelpers.UpdateCountLevelUpEquipment(playerId, playerAchievements, out createAchievements, out updateAchievements);
                    foreach (var createEntry in createAchievements)
                    {
                        QueryCreatePlayerAchievement(createEntry);
                    }
                    foreach (var updateEntry in updateAchievements)
                    {
                        QueryUpdatePlayerAchievement(updateEntry);
                    }
                }
            }
        }
        onFinish(result);
    }

    protected override void DoEvolveItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = GetPlayerByLoginToken(playerId, loginToken);
        var foundItem = GetPlayerItemById(itemId);
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundItem == null || foundItem.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            if (!foundItem.CanEvolve)
                result.error = GameServiceErrorCode.CANNOT_EVOLVE;
            else
            {
                var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
                var requireCurrency = 0;
                var itemData = foundItem.ItemData;
                requireCurrency = foundItem.EvolvePrice;
                List<PlayerItem> updateItems;
                List<string> deleteItemIds;
                var enoughMaterials = HaveEnoughMaterials(playerId, materials, foundItem.EvolveMaterials, out updateItems, out deleteItemIds);
                if (requireCurrency > softCurrency.Amount)
                    result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
                else if (!enoughMaterials)
                    result.error = GameServiceErrorCode.NOT_ENOUGH_ITEMS;
                else
                {
                    softCurrency.Amount -= requireCurrency;
                    ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                        new SqliteParameter("@amount", softCurrency.Amount),
                        new SqliteParameter("@id", softCurrency.Id));
                    foundItem = foundItem.CreateEvolveItem();
                    updateItems.Add(foundItem);
                    foreach (var updateItem in updateItems)
                    {
                        QueryUpdatePlayerItem(updateItem);
                    }
                    foreach (var deleteItemId in deleteItemIds)
                    {
                        ExecuteNonQuery(@"DELETE FROM playerItem WHERE id=@id", new SqliteParameter("@id", deleteItemId));
                    }
                    result.updateCurrencies.Add(softCurrency);
                    result.updateItems = updateItems;
                    result.deleteItemIds = deleteItemIds;
                    // Update achievement
                    if (foundItem.ActorItemData.Type.Equals("CharacterItem") ||
                        foundItem.ActorItemData.Type.Equals("EquipmentItem"))
                    {
                        var playerAchievements = GetPlayerAchievements(playerId);
                        List<PlayerAchievement> createAchievements = null;
                        List<PlayerAchievement> updateAchievements = null;
                        if (foundItem.ActorItemData.Type.Equals("CharacterItem"))
                            OfflineAchievementHelpers.UpdateCountEvolveCharacter(playerId, playerAchievements, out createAchievements, out updateAchievements);
                        if (foundItem.ActorItemData.Type.Equals("EquipmentItem"))
                            OfflineAchievementHelpers.UpdateCountEvolveEquipment(playerId, playerAchievements, out createAchievements, out updateAchievements);
                        foreach (var createEntry in createAchievements)
                        {
                            QueryCreatePlayerAchievement(createEntry);
                        }
                        foreach (var updateEntry in updateAchievements)
                        {
                            QueryUpdatePlayerAchievement(updateEntry);
                        }
                    }
                }
            }
        }
        onFinish(result);
    }

    protected override void DoSellItems(string playerId, string loginToken, Dictionary<string, int> items, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var player = GetPlayerByLoginToken(playerId, loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
        {
            var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
            var returnCurrency = 0;
            var updateItems = new List<PlayerItem>();
            var deleteItemIds = new List<string>();
            var sellingItemIds = items.Keys;
            var sellingItems = new List<PlayerItem>();
            foreach (var sellingItemId in sellingItemIds)
            {
                var foundItem = GetPlayerItemById(sellingItemId);
                if (foundItem == null || foundItem.PlayerId != playerId)
                    continue;
                
                if (foundItem.CanSell)
                    sellingItems.Add(foundItem);
            }
            foreach (var sellingItem in sellingItems)
            {
                var usingAmount = items[sellingItem.Id];
                if (usingAmount > sellingItem.Amount)
                    usingAmount = sellingItem.Amount;
                returnCurrency += sellingItem.SellPrice * usingAmount;
                sellingItem.Amount -= usingAmount;
                if (sellingItem.Amount > 0)
                    updateItems.Add(sellingItem);
                else
                    deleteItemIds.Add(sellingItem.Id);
            }
            softCurrency.Amount += returnCurrency;
            ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                new SqliteParameter("@amount", softCurrency.Amount),
                new SqliteParameter("@id", softCurrency.Id));
            foreach (var updateItem in updateItems)
            {
                ExecuteNonQuery(@"UPDATE playerItem SET playerId=@playerId, dataId=@dataId, amount=@amount, exp=@exp, equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                    new SqliteParameter("@playerId", updateItem.PlayerId),
                    new SqliteParameter("@dataId", updateItem.DataId),
                    new SqliteParameter("@amount", updateItem.Amount),
                    new SqliteParameter("@exp", updateItem.Exp),
                    new SqliteParameter("@equipItemId", updateItem.EquipItemId),
                    new SqliteParameter("@equipPosition", updateItem.EquipPosition),
                    new SqliteParameter("@id", updateItem.Id));
            }
            foreach (var deleteItemId in deleteItemIds)
            {
                ExecuteNonQuery(@"DELETE FROM playerItem WHERE id=@id", new SqliteParameter("@id", deleteItemId));
            }
            result.updateCurrencies.Add(softCurrency);
            result.updateItems = updateItems;
            result.deleteItemIds = deleteItemIds;
        }
        onFinish(result);
    }

    protected override void DoEquipItem(string playerId, string loginToken, string characterId, string equipmentId, string equipPosition, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = GetPlayerByLoginToken(playerId, loginToken);
        var foundCharacter = GetPlayerItemById(characterId);
        var foundEquipment = GetPlayerItemById(equipmentId);
        CharacterItem characterData = null;
        EquipmentItem equipmentData = null;
        if (foundCharacter != null)
            characterData = foundCharacter.CharacterData;
        if (foundEquipment != null)
            equipmentData = foundEquipment.EquipmentData;
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundCharacter == null || foundCharacter.PlayerId != playerId || foundEquipment == null || foundEquipment.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else if (characterData == null || equipmentData == null)
            result.error = GameServiceErrorCode.INVALID_ITEM_DATA;
        else if (!equipmentData.equippablePositions.Contains(equipPosition))
            result.error = GameServiceErrorCode.INVALID_EQUIP_POSITION;
        else
        {
            result.updateItems = new List<PlayerItem>();
            var unEquipItem = GetPlayerItemByEquipper(playerId, characterId, equipPosition);
            if (unEquipItem != null)
            {
                unEquipItem.EquipItemId = "";
                unEquipItem.EquipPosition = "";
                ExecuteNonQuery(@"UPDATE playerItem SET equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                    new SqliteParameter("@equipItemId", unEquipItem.EquipItemId),
                    new SqliteParameter("@equipPosition", unEquipItem.EquipPosition),
                    new SqliteParameter("@id", unEquipItem.Id));
                result.updateItems.Add(unEquipItem);
            }
            foundEquipment.EquipItemId = characterId;
            foundEquipment.EquipPosition = equipPosition;
            ExecuteNonQuery(@"UPDATE playerItem SET equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                new SqliteParameter("@equipItemId", foundEquipment.EquipItemId),
                new SqliteParameter("@equipPosition", foundEquipment.EquipPosition),
                new SqliteParameter("@id", foundEquipment.Id));
            result.updateItems.Add(foundEquipment);
        }
        onFinish(result);
    }

    protected override void DoUnEquipItem(string playerId, string loginToken, string equipmentId, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var player = GetPlayerByLoginToken(playerId, loginToken);
        var unEquipItem = GetPlayerItemById(equipmentId);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (unEquipItem == null || unEquipItem.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            result.updateItems = new List<PlayerItem>();
            unEquipItem.EquipItemId = "";
            unEquipItem.EquipPosition = "";
            ExecuteNonQuery(@"UPDATE playerItem SET equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                new SqliteParameter("@equipItemId", unEquipItem.EquipItemId),
                new SqliteParameter("@equipPosition", unEquipItem.EquipPosition),
                new SqliteParameter("@id", unEquipItem.Id));
            result.updateItems.Add(unEquipItem);
        }
        onFinish(result);
    }

    protected override void DoGetAvailableLootBoxList(UnityAction<AvailableLootBoxListResult> onFinish)
    {
        var result = new AvailableLootBoxListResult();
        var gameDb = GameInstance.GameDatabase;
        result.list.AddRange(gameDb.LootBoxes.Keys);
        onFinish(result);
    }

    protected override void DoGetAvailableIapPackageList(UnityAction<AvailableIapPackageListResult> onFinish)
    {
        var result = new AvailableIapPackageListResult();
        var gameDb = GameInstance.GameDatabase;
        result.list.AddRange(gameDb.IapPackages.Keys);
        onFinish(result);
    }

    protected override void DoGetAvailableInGamePackageList(UnityAction<AvailableInGamePackageListResult> onFinish)
    {
        var result = new AvailableInGamePackageListResult();
        var gameDb = GameInstance.GameDatabase;
        result.list.AddRange(gameDb.InGamePackages.Keys);
        onFinish(result);
    }

    protected override void DoOpenLootBox(string playerId, string loginToken, string lootBoxDataId, int packIndex, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        LootBox lootBox;
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.LootBoxes.TryGetValue(lootBoxDataId, out lootBox))
            result.error = GameServiceErrorCode.INVALID_LOOT_BOX_DATA;
        else
        {
            var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            var requirementType = lootBox.requirementType;
            if (packIndex > lootBox.lootboxPacks.Length - 1)
                packIndex = 0;
            var pack = lootBox.lootboxPacks[packIndex];
            var price = pack.price;
            var openAmount = pack.openAmount;
            if (requirementType == LootBoxRequirementType.RequireSoftCurrency && price > softCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
            else if (requirementType == LootBoxRequirementType.RequireHardCurrency && price > hardCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY;
            else
            {
                switch (requirementType)
                {
                    case LootBoxRequirementType.RequireSoftCurrency:
                        softCurrency.Amount -= price;
                        ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                            new SqliteParameter("@amount", softCurrency.Amount),
                            new SqliteParameter("@id", softCurrency.Id));
                        result.updateCurrencies.Add(softCurrency);
                        break;
                    case LootBoxRequirementType.RequireHardCurrency:
                        hardCurrency.Amount -= price;
                        ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                            new SqliteParameter("@amount", hardCurrency.Amount),
                            new SqliteParameter("@id", hardCurrency.Id));
                        result.updateCurrencies.Add(hardCurrency);
                        break;
                }

                for (var i = 0; i < openAmount; ++i)
                {
                    var rewardItem = lootBox.RandomReward().rewardItem;
                    var createItems = new List<PlayerItem>();
                    var updateItems = new List<PlayerItem>();
                    if (AddItems(playerId, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                    {
                        result.rewardItems.Add(new PlayerItem()
                        {
                            Id = i.ToString(),
                            PlayerId = player.Id,
                            DataId = rewardItem.Id,
                            Amount = rewardItem.amount
                        });
                        foreach (var createEntry in createItems)
                        {
                            QueryCreatePlayerItem(createEntry);
                            result.createItems.Add(createEntry);
                            HelperUnlockItem(player.Id, createEntry.DataId);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            QueryUpdatePlayerItem(updateEntry);
                            result.updateItems.Add(updateEntry);
                        }
                    }
                }
            }
        }
        onFinish(result);
    }

    protected override void DoOpenIapPackage_iOS(string playerId, string loginToken, string iapPackageDataId, string receipt, UnityAction<ItemResult> onFinish)
    {
        // Don't validate IAP for offline services
        DoOpenIAPPackage(playerId, loginToken, iapPackageDataId, onFinish);
    }

    protected override void DoOpenIapPackage_Android(string playerId, string loginToken, string iapPackageDataId, string data, string signature, UnityAction<ItemResult> onFinish)
    {
        // Don't validate IAP for offline services
        DoOpenIAPPackage(playerId, loginToken, iapPackageDataId, onFinish);
    }

    protected void DoOpenIAPPackage(string playerId, string loginToken, string iapPackageDataId, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        IapPackage iapPackage;
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.IapPackages.TryGetValue(iapPackageDataId, out iapPackage))
            result.error = GameServiceErrorCode.INVALID_IAP_PACKAGE_DATA;
        else
        {
            // Add soft currency
            var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
            softCurrency.Amount += iapPackage.rewardSoftCurrency;
            ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                new SqliteParameter("@amount", softCurrency.Amount),
                new SqliteParameter("@id", softCurrency.Id));
            result.updateCurrencies.Add(softCurrency);
            // Add hard currency
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            hardCurrency.Amount += iapPackage.rewardHardCurrency;
            ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                new SqliteParameter("@amount", hardCurrency.Amount),
                new SqliteParameter("@id", hardCurrency.Id));
            result.updateCurrencies.Add(hardCurrency);
            // Add items
            for (var i = 0; i < iapPackage.rewardItems.Length; ++i)
            {
                var rewardItem = iapPackage.rewardItems[i];
                var createItems = new List<PlayerItem>();
                var updateItems = new List<PlayerItem>();
                if (AddItems(playerId, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                {
                    result.rewardItems.Add(new PlayerItem()
                    {
                        Id = i.ToString(),
                        PlayerId = player.Id,
                        DataId = rewardItem.Id,
                        Amount = rewardItem.amount
                    });
                    foreach (var createEntry in createItems)
                    {
                        QueryCreatePlayerItem(createEntry);
                        result.createItems.Add(createEntry);
                        HelperUnlockItem(player.Id, createEntry.DataId);
                    }
                    foreach (var updateEntry in updateItems)
                    {
                        QueryUpdatePlayerItem(updateEntry);
                        result.updateItems.Add(updateEntry);
                    }
                }
            }
        }
        onFinish(result);
    }

    protected override void DoEarnAchievementReward(string playerId, string loginToken, string achievementId, UnityAction<EarnAchievementResult> onFinish)
    {
        var result = new EarnAchievementResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        Achievement achievement;
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.Achievements.TryGetValue(achievementId, out achievement))
            result.error = GameServiceErrorCode.INVALID_ACHIEVEMENT_DATA;
        else
        {
            var playerAchievement = GetPlayerAchievement(playerId, achievement.Id);
            if (playerAchievement == null)
                result.error = GameServiceErrorCode.ACHIEVEMENT_UNDONE;
            else if (playerAchievement.Earned)
                result.error = GameServiceErrorCode.ACHIEVEMENT_EARNED;
            else if (playerAchievement.Progress < achievement.targetAmount)
                result.error = GameServiceErrorCode.ACHIEVEMENT_UNDONE;
            else
            {
                // Player exp
                result.rewardPlayerExp = achievement.rewardPlayerExp;
                player.Exp += achievement.rewardPlayerExp;
                ExecuteNonQuery(@"UPDATE player SET exp=@exp WHERE id=@playerId",
                    new SqliteParameter("@exp", player.Exp),
                    new SqliteParameter("@playerId", player.Id));
                result.player = player;
                // Add soft currency
                var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                result.rewardSoftCurrency = achievement.rewardSoftCurrency;
                softCurrency.Amount += achievement.rewardSoftCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", softCurrency.Amount),
                    new SqliteParameter("@id", softCurrency.Id));
                result.updateCurrencies.Add(softCurrency);
                // Add hard currency
                var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
                result.rewardHardCurrency = achievement.rewardHardCurrency;
                hardCurrency.Amount += achievement.rewardHardCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", hardCurrency.Amount),
                    new SqliteParameter("@id", hardCurrency.Id));
                result.updateCurrencies.Add(hardCurrency);
                // Add items
                for (var i = 0; i < achievement.rewardItems.Length; ++i)
                {
                    var rewardItem = achievement.rewardItems[i];
                    var createItems = new List<PlayerItem>();
                    var updateItems = new List<PlayerItem>();
                    if (AddItems(playerId, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                    {
                        result.rewardItems.Add(new PlayerItem()
                        {
                            Id = i.ToString(),
                            PlayerId = player.Id,
                            DataId = rewardItem.Id,
                            Amount = rewardItem.amount
                        });
                        foreach (var createEntry in createItems)
                        {
                            QueryCreatePlayerItem(createEntry);
                            result.createItems.Add(createEntry);
                            HelperUnlockItem(player.Id, createEntry.DataId);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            QueryUpdatePlayerItem(updateEntry);
                            result.updateItems.Add(updateEntry);
                        }
                    }
                }
                // Update achievement status
                playerAchievement.Earned = true;
                QueryUpdatePlayerAchievement(playerAchievement);
            }
        }
        onFinish(result);
    }

    protected override void DoConvertHardCurrency(string playerId, string loginToken, int requireHardCurrency, UnityAction<HardCurrencyConversionResult> onFinish)
    {
        var result = new HardCurrencyConversionResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
        {
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            if (hardCurrency.Amount < requireHardCurrency)
                result.error = GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY;
            else
            {
                // Decrease hard currency
                result.requireHardCurrency = requireHardCurrency;
                hardCurrency.Amount -= requireHardCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", hardCurrency.Amount),
                    new SqliteParameter("@id", hardCurrency.Id));
                result.updateCurrencies.Add(hardCurrency);
                // Increase soft currency
                var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                result.receiveSoftCurrency = gameDb.hardToSoftCurrencyConversion * requireHardCurrency;
                softCurrency.Amount += result.receiveSoftCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", softCurrency.Amount),
                    new SqliteParameter("@id", softCurrency.Id));
                result.updateCurrencies.Add(softCurrency);
            }
        }
        onFinish(result);
    }

    protected override void DoOpenInGamePackage(string playerId, string loginToken, string inGamePackageDataId, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        InGamePackage inGamePackage;
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.InGamePackages.TryGetValue(inGamePackageDataId, out inGamePackage))
            result.error = GameServiceErrorCode.INVALID_IN_GAME_PACKAGE_DATA;
        else
        {
            var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            var requirementType = inGamePackage.requirementType;
            var price = inGamePackage.price;
            if (requirementType == InGamePackageRequirementType.RequireSoftCurrency && price > softCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
            else if (requirementType == InGamePackageRequirementType.RequireHardCurrency && price > hardCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY;
            else
            {
                switch (requirementType)
                {
                    case InGamePackageRequirementType.RequireSoftCurrency:
                        softCurrency.Amount -= price;
                        break;
                    case InGamePackageRequirementType.RequireHardCurrency:
                        hardCurrency.Amount -= price;
                        break;
                }
                // Add soft currency
                softCurrency.Amount += inGamePackage.rewardSoftCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", softCurrency.Amount),
                    new SqliteParameter("@id", softCurrency.Id));
                result.updateCurrencies.Add(softCurrency);
                // Add hard currency
                hardCurrency.Amount += inGamePackage.rewardHardCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", hardCurrency.Amount),
                    new SqliteParameter("@id", hardCurrency.Id));
                result.updateCurrencies.Add(hardCurrency);
                // Add items
                for (var i = 0; i < inGamePackage.rewardItems.Length; ++i)
                {
                    var rewardItem = inGamePackage.rewardItems[i];
                    List<PlayerItem> createItems;
                    List<PlayerItem> updateItems;
                    if (AddItems(playerId, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                    {
                        result.rewardItems.Add(new PlayerItem()
                        {
                            Id = i.ToString(),
                            PlayerId = player.Id,
                            DataId = rewardItem.Id,
                            Amount = rewardItem.amount
                        });
                        foreach (var createEntry in createItems)
                        {
                            QueryCreatePlayerItem(createEntry);
                            result.createItems.Add(createEntry);
                            HelperUnlockItem(player.Id, createEntry.DataId);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            QueryUpdatePlayerItem(updateEntry);
                            result.updateItems.Add(updateEntry);
                        }
                    }
                }
            }
        }
        onFinish(result);
    }

    protected override void DoCraftItem(string playerId, string loginToken, string itemCraftId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        ItemCraftFormula itemCraft;
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.ItemCrafts.TryGetValue(itemCraftId, out itemCraft))
            result.error = GameServiceErrorCode.INVALID_ITEM_CRAFT_FORMULA_DATA;
        else
        {
            var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            var requirementType = itemCraft.requirementType;
            var price = itemCraft.price;
            List<PlayerItem> queryUpdateItems;
            List<string> queryDeleteItemIds;
            var enoughMaterials = HaveEnoughMaterials(playerId, materials, itemCraft.CacheMaterials, out queryUpdateItems, out queryDeleteItemIds);
            if (requirementType == CraftRequirementType.RequireSoftCurrency && price > softCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
            else if (requirementType == CraftRequirementType.RequireHardCurrency && price > hardCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY;
            else if (!enoughMaterials)
                result.error = GameServiceErrorCode.NOT_ENOUGH_ITEMS;
            else
            {
                // Query items
                result.updateItems = queryUpdateItems;
                result.deleteItemIds = queryDeleteItemIds;
                foreach (var updateItem in queryUpdateItems)
                {
                    QueryUpdatePlayerItem(updateItem);
                }
                foreach (var deleteItemId in queryDeleteItemIds)
                {
                    ExecuteNonQuery(@"DELETE FROM playerItem WHERE id=@id", new SqliteParameter("@id", deleteItemId));
                }
                // Update currencies
                switch (requirementType)
                {
                    case CraftRequirementType.RequireSoftCurrency:
                        softCurrency.Amount -= price;
                        break;
                    case CraftRequirementType.RequireHardCurrency:
                        hardCurrency.Amount -= price;
                        break;
                }
                // Update soft currency
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", softCurrency.Amount),
                    new SqliteParameter("@id", softCurrency.Id));
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                // Update hard currency
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", hardCurrency.Amount),
                    new SqliteParameter("@id", hardCurrency.Id));
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
                // Add items
                List<PlayerItem> createItems;
                List<PlayerItem> updateItems;
                if (AddItems(playerId, itemCraft.resultItem.Id, itemCraft.resultItem.amount, out createItems, out updateItems))
                {
                    result.rewardItems.Add(new PlayerItem()
                    {
                        Id = "Result",
                        PlayerId = player.Id,
                        DataId = itemCraft.resultItem.Id,
                        Amount = itemCraft.resultItem.amount
                    });
                    foreach (var createEntry in createItems)
                    {
                        QueryCreatePlayerItem(createEntry);
                        result.createItems.Add(createEntry);
                        HelperUnlockItem(player.Id, createEntry.DataId);
                    }
                    foreach (var updateEntry in updateItems)
                    {
                        QueryUpdatePlayerItem(updateEntry);
                        result.updateItems.Add(updateEntry);
                    }
                }
            }
        }
        onFinish(result);
    }
}
