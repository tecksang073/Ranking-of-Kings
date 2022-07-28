using System.Collections.Generic;
using UnityEngine.Events;

public partial class LiteDbGameService
{
    protected override void DoLevelUpItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        var foundItem = colPlayerItem.FindOne(a => a.Id == itemId && a.PlayerId == playerId);
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundItem == null)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            var item = PlayerItem.CloneTo(foundItem, new PlayerItem());
            var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
            var levelUpPrice = item.LevelUpPrice;
            var requireCurrency = 0;
            var increasingExp = 0;
            var updateItems = new List<PlayerItem>();
            var deleteItemIds = new List<string>();
            var materialItemIds = materials.Keys;
            var materialItems = new List<PlayerItem>();
            foreach (var materialItemId in materialItemIds)
            {
                var foundMaterial = colPlayerItem.FindOne(a => a.Id == materialItemId && a.PlayerId == playerId);
                if (foundMaterial == null)
                    continue;

                var resultItem = PlayerItem.CloneTo(foundMaterial, new PlayerItem());
                if (resultItem.CanBeMaterial)
                    materialItems.Add(resultItem);
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
                colPlayer.Update(foundPlayer);
                colPlayerCurrency.Update(softCurrency);
                item = item.CreateLevelUpItem(increasingExp);
                updateItems.Add(item);
                foreach (var updateItem in updateItems)
                {
                    colPlayerItem.Update(PlayerItem.CloneTo(updateItem, new DbPlayerItem()));
                }
                foreach (var deleteItemId in deleteItemIds)
                {
                    colPlayerItem.Delete(deleteItemId);
                }
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                result.updateItems = updateItems;
                result.deleteItemIds = deleteItemIds;
                // Update achievement
                if (item.ActorItemData.Type.Equals("CharacterItem") ||
                    item.ActorItemData.Type.Equals("EquipmentItem"))
                {
                    var playerAchievements = DbPlayerAchievement.CloneList(colPlayerAchievement.Find(a => a.PlayerId == playerId));
                    List<PlayerAchievement> createAchievements = null;
                    List<PlayerAchievement> updateAchievements = null;
                    if (item.ActorItemData.Type.Equals("CharacterItem"))
                        OfflineAchievementHelpers.UpdateCountLevelUpCharacter(playerId, playerAchievements, out createAchievements, out updateAchievements);
                    if (item.ActorItemData.Type.Equals("EquipmentItem"))
                        OfflineAchievementHelpers.UpdateCountLevelUpEquipment(playerId, playerAchievements, out createAchievements, out updateAchievements);
                    foreach (var createEntry in createAchievements)
                    {
                        createEntry.Id = System.Guid.NewGuid().ToString();
                        colPlayerAchievement.Insert(PlayerAchievement.CloneTo(createEntry, new DbPlayerAchievement()));
                    }
                    foreach (var updateEntry in updateAchievements)
                    {
                        colPlayerAchievement.Update(PlayerAchievement.CloneTo(updateEntry, new DbPlayerAchievement()));
                    }
                }
            }
        }
        onFinish(result);
    }

    protected override void DoEvolveItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        var foundItem = colPlayerItem.FindOne(a => a.Id == itemId && a.PlayerId == playerId);
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundItem == null)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            var item = PlayerItem.CloneTo(foundItem, new PlayerItem());

            if (!item.CanEvolve)
                result.error = GameServiceErrorCode.CANNOT_EVOLVE;
            else
            {
                var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
                var requireCurrency = 0;
                var itemData = item.ItemData;
                requireCurrency = item.EvolvePrice;
                List<PlayerItem> updateItems;
                List<string> deleteItemIds;
                var enoughMaterials = HaveEnoughMaterials(playerId, materials, item.EvolveMaterials, out updateItems, out deleteItemIds);
                if (requireCurrency > softCurrency.Amount)
                    result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
                else if (!enoughMaterials)
                    result.error = GameServiceErrorCode.NOT_ENOUGH_ITEMS;
                else
                {
                    softCurrency.Amount -= requireCurrency;
                    colPlayer.Update(foundPlayer);
                    colPlayerCurrency.Update(softCurrency);
                    item = item.CreateEvolveItem();
                    updateItems.Add(item);
                    foreach (var updateItem in updateItems)
                    {
                        colPlayerItem.Update(PlayerItem.CloneTo(updateItem, new DbPlayerItem()));
                    }
                    foreach (var deleteItemId in deleteItemIds)
                    {
                        colPlayerItem.Delete(deleteItemId);
                    }
                    result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                    result.updateItems = updateItems;
                    result.deleteItemIds = deleteItemIds;
                    // Update achievement
                    if (item.ActorItemData.Type.Equals("CharacterItem") ||
                        item.ActorItemData.Type.Equals("EquipmentItem"))
                    {
                        var playerAchievements = DbPlayerAchievement.CloneList(colPlayerAchievement.Find(a => a.PlayerId == playerId));
                        List<PlayerAchievement> createAchievements = null;
                        List<PlayerAchievement> updateAchievements = null;
                        if (item.ActorItemData.Type.Equals("CharacterItem"))
                            OfflineAchievementHelpers.UpdateCountEvolveCharacter(playerId, playerAchievements, out createAchievements, out updateAchievements);
                        if (item.ActorItemData.Type.Equals("EquipmentItem"))
                            OfflineAchievementHelpers.UpdateCountEvolveEquipment(playerId, playerAchievements, out createAchievements, out updateAchievements);
                        foreach (var createEntry in createAchievements)
                        {
                            createEntry.Id = System.Guid.NewGuid().ToString();
                            colPlayerAchievement.Insert(PlayerAchievement.CloneTo(createEntry, new DbPlayerAchievement()));
                        }
                        foreach (var updateEntry in updateAchievements)
                        {
                            colPlayerAchievement.Update(PlayerAchievement.CloneTo(updateEntry, new DbPlayerAchievement()));
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
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
                var foundItem = colPlayerItem.FindOne(a => a.Id == sellingItemId && a.PlayerId == playerId);
                if (foundItem == null)
                    continue;

                var resultItem = PlayerItem.CloneTo(foundItem, new PlayerItem());
                if (resultItem.CanSell)
                    sellingItems.Add(resultItem);
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
            colPlayer.Update(player);
            colPlayerCurrency.Update(softCurrency);
            foreach (var updateItem in updateItems)
            {
                colPlayerItem.Update(PlayerItem.CloneTo(updateItem, new DbPlayerItem()));
            }
            foreach (var deleteItemId in deleteItemIds)
            {
                colPlayerItem.Delete(deleteItemId);
            }
            result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
            result.updateItems = updateItems;
            result.deleteItemIds = deleteItemIds;
        }
        onFinish(result);
    }

    protected override void DoEquipItem(string playerId, string loginToken, string characterId, string equipmentId, string equipPosition, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        var foundCharacter = colPlayerItem.FindOne(a => a.Id == characterId && a.PlayerId == playerId);
        var foundEquipment = colPlayerItem.FindOne(a => a.Id == equipmentId && a.PlayerId == playerId);
        CharacterItem characterData = null;
        EquipmentItem equipmentData = null;
        if (foundCharacter != null)
        {
            var character = PlayerItem.CloneTo(foundCharacter, new PlayerItem());
            characterData = character.CharacterData;
        }
        if (foundEquipment != null)
        {
            var equipment = PlayerItem.CloneTo(foundEquipment, new PlayerItem());
            equipmentData = equipment.EquipmentData;
        }
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundCharacter == null || foundEquipment == null)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else if (characterData == null || equipmentData == null)
            result.error = GameServiceErrorCode.INVALID_ITEM_DATA;
        else if (!equipmentData.equippablePositions.Contains(equipPosition))
            result.error = GameServiceErrorCode.INVALID_EQUIP_POSITION;
        else
        {
            result.updateItems = new List<PlayerItem>();
            var unEquipItem = colPlayerItem.FindOne(a => a.EquipItemId == characterId && a.EquipPosition == equipPosition && a.PlayerId == playerId);
            if (unEquipItem != null)
            {
                unEquipItem.EquipItemId = "";
                unEquipItem.EquipPosition = "";
                colPlayerItem.Update(unEquipItem);
                result.updateItems.Add(PlayerItem.CloneTo(unEquipItem, new PlayerItem()));
            }
            foundEquipment.EquipItemId = characterId;
            foundEquipment.EquipPosition = equipPosition;
            colPlayerItem.Update(foundEquipment);
            result.updateItems.Add(PlayerItem.CloneTo(foundEquipment, new PlayerItem()));
        }
        onFinish(result);
    }

    protected override void DoUnEquipItem(string playerId, string loginToken, string equipmentId, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        var unEquipItem = colPlayerItem.FindOne(a => a.Id == equipmentId && a.PlayerId == playerId);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (unEquipItem == null)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            result.updateItems = new List<PlayerItem>();
            unEquipItem.EquipItemId = "";
            unEquipItem.EquipPosition = "";
            colPlayerItem.Update(unEquipItem);
            result.updateItems.Add(PlayerItem.CloneTo(unEquipItem, new PlayerItem()));
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
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
                        colPlayerCurrency.Update(softCurrency);
                        result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                        break;
                    case LootBoxRequirementType.RequireHardCurrency:
                        hardCurrency.Amount -= price;
                        colPlayerCurrency.Update(hardCurrency);
                        result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
                        break;
                }

                for (var i = 0; i < openAmount; ++i)
                {
                    var rewardItem = lootBox.RandomReward().rewardItem;
                    var createItems = new List<DbPlayerItem>();
                    var updateItems = new List<DbPlayerItem>();
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
                            createEntry.Id = System.Guid.NewGuid().ToString();
                            colPlayerItem.Insert(createEntry);
                            result.createItems.Add(PlayerItem.CloneTo(createEntry, new PlayerItem()));
                            HelperUnlockItem(player.Id, createEntry.DataId);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            colPlayerItem.Update(updateEntry);
                            result.updateItems.Add(PlayerItem.CloneTo(updateEntry, new PlayerItem()));
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
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
            colPlayerCurrency.Update(softCurrency);
            result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
            // Add hard currency
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            hardCurrency.Amount += iapPackage.rewardHardCurrency;
            colPlayerCurrency.Update(hardCurrency);
            result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
            // Add items
            for (var i = 0; i < iapPackage.rewardItems.Length; ++i)
            {
                var rewardItem = iapPackage.rewardItems[i];
                var createItems = new List<DbPlayerItem>();
                var updateItems = new List<DbPlayerItem>();
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
                        createEntry.Id = System.Guid.NewGuid().ToString();
                        colPlayerItem.Insert(createEntry);
                        result.createItems.Add(PlayerItem.CloneTo(createEntry, new PlayerItem()));
                        HelperUnlockItem(player.Id, createEntry.DataId);
                    }
                    foreach (var updateEntry in updateItems)
                    {
                        colPlayerItem.Update(updateEntry);
                        result.updateItems.Add(PlayerItem.CloneTo(updateEntry, new PlayerItem()));
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
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        Achievement achievement;
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.Achievements.TryGetValue(achievementId, out achievement))
            result.error = GameServiceErrorCode.INVALID_ACHIEVEMENT_DATA;
        else
        {
            var playerAchievement = colPlayerAchievement.FindOne(a => a.PlayerId == playerId && a.DataId == achievement.Id);
            if (playerAchievement == null)
                result.error = GameServiceErrorCode.ACHIEVEMENT_UNDONE;
            if (playerAchievement.Earned)
                result.error = GameServiceErrorCode.ACHIEVEMENT_EARNED;
            else if (playerAchievement.Progress < achievement.targetAmount)
                result.error = GameServiceErrorCode.ACHIEVEMENT_UNDONE;
            else
            {
                // Player exp
                result.rewardPlayerExp = achievement.rewardPlayerExp;
                player.Exp += achievement.rewardPlayerExp;
                colPlayer.Update(player);
                result.player = Player.CloneTo(player, new Player());
                // Add soft currency
                var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                result.rewardSoftCurrency = achievement.rewardSoftCurrency;
                softCurrency.Amount += achievement.rewardSoftCurrency;
                colPlayerCurrency.Update(softCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                // Add hard currency
                var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
                result.rewardHardCurrency = achievement.rewardHardCurrency;
                hardCurrency.Amount += achievement.rewardHardCurrency;
                colPlayerCurrency.Update(hardCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                // Add items
                for (var i = 0; i < achievement.rewardItems.Length; ++i)
                {
                    var rewardItem = achievement.rewardItems[i];
                    var createItems = new List<DbPlayerItem>();
                    var updateItems = new List<DbPlayerItem>();
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
                            createEntry.Id = System.Guid.NewGuid().ToString();
                            colPlayerItem.Insert(createEntry);
                            var resultItem = PlayerItem.CloneTo(createEntry, new PlayerItem());
                            result.createItems.Add(resultItem);
                            HelperUnlockItem(player.Id, createEntry.DataId);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            colPlayerItem.Update(updateEntry);
                            var resultItem = PlayerItem.CloneTo(updateEntry, new PlayerItem());
                            result.updateItems.Add(resultItem);
                        }
                    }
                }
                // Update achievement status
                playerAchievement.Earned = true;
                colPlayerAchievement.Update(PlayerAchievement.CloneTo(playerAchievement, new DbPlayerAchievement()));
            }
        }
        onFinish(result);
    }

    protected override void DoConvertHardCurrency(string playerId, string loginToken, int requireHardCurrency, UnityAction<HardCurrencyConversionResult> onFinish)
    {
        var result = new HardCurrencyConversionResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
                colPlayerCurrency.Update(hardCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
                // Increase soft currency
                var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                result.receiveSoftCurrency = gameDb.hardToSoftCurrencyConversion * requireHardCurrency;
                softCurrency.Amount += result.receiveSoftCurrency;
                colPlayerCurrency.Update(softCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
            }
        }
        onFinish(result);
    }

    protected override void DoOpenInGamePackage(string playerId, string loginToken, string inGamePackageDataId, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
                colPlayerCurrency.Update(softCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                // Add hard currency
                hardCurrency.Amount += inGamePackage.rewardHardCurrency;
                colPlayerCurrency.Update(hardCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
                // Add items
                for (var i = 0; i < inGamePackage.rewardItems.Length; ++i)
                {
                    var rewardItem = inGamePackage.rewardItems[i];
                    List<DbPlayerItem> createItems;
                    List<DbPlayerItem> updateItems;
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
                            createEntry.Id = System.Guid.NewGuid().ToString();
                            colPlayerItem.Insert(createEntry);
                            result.createItems.Add(PlayerItem.CloneTo(createEntry, new PlayerItem()));
                            HelperUnlockItem(player.Id, createEntry.DataId);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            colPlayerItem.Update(updateEntry);
                            result.updateItems.Add(PlayerItem.CloneTo(updateEntry, new PlayerItem()));
                        }
                    }
                }
            }
            onFinish(result);
        }
    }

    protected override void DoCraftItem(string playerId, string loginToken, string itemCraftId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
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
                    colPlayerItem.Update(PlayerItem.CloneTo(updateItem, new DbPlayerItem()));
                }
                foreach (var deleteItemId in queryDeleteItemIds)
                {
                    colPlayerItem.Delete(deleteItemId);
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
                colPlayerCurrency.Update(softCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(softCurrency, new PlayerCurrency()));
                // Update hard currency
                colPlayerCurrency.Update(hardCurrency);
                result.updateCurrencies.Add(PlayerCurrency.CloneTo(hardCurrency, new PlayerCurrency()));
                // Add items
                var createItems = new List<DbPlayerItem>();
                var updateItems = new List<DbPlayerItem>();
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
                        createEntry.Id = System.Guid.NewGuid().ToString();
                        colPlayerItem.Insert(createEntry);
                        result.createItems.Add(PlayerItem.CloneTo(createEntry, new PlayerItem()));
                        HelperUnlockItem(player.Id, createEntry.DataId);
                    }
                    foreach (var updateEntry in updateItems)
                    {
                        colPlayerItem.Update(updateEntry);
                        result.updateItems.Add(PlayerItem.CloneTo(updateEntry, new PlayerItem()));
                    }
                }
            }
        }
        onFinish(result);
    }
}
