[System.Serializable]
public struct UIItemListFilterSetting
{
    public bool showCharacter;
    public bool showEquipment;
    public bool showMaterial;
    public bool dontShowInTeamCharacter;
    public bool dontShowEquippedEquipment;
    public string category;
}

public static class UIItemListFilter
{
    public static bool Filter(PlayerItem item, UIItemListFilterSetting setting)
    {
        if (!string.IsNullOrEmpty(setting.category) && !MatchCategory(item, setting.category))
            return false;
        if (IsMaterial(item))
            return setting.showMaterial;
        if (IsCharacter(item))
        {
            if (!setting.showCharacter)
                return false;
            if (setting.dontShowInTeamCharacter && item.InTeamFormations.Count > 0)
                return false;
        }
        if (IsEquipment(item))
        {
            if (!setting.showEquipment)
                return false;
            if (setting.dontShowEquippedEquipment && item.EquippedByItem != null)
                return false;
        }
        return true;
    }

    public static bool IsCharacter(PlayerItem item)
    {
        return item != null && item.CharacterData != null;
    }

    public static bool IsEquipment(PlayerItem item)
    {
        return item != null && item.EquipmentData != null;
    }

    public static bool IsMaterial(PlayerItem item)
    {
        return item != null && item.MaterialData != null && !IsCharacter(item) && !IsEquipment(item);
    }

    public static bool MatchCategory(PlayerItem item, string category)
    {
        return item != null && item.ItemData != null && item.ItemData.category == category;
    }
}
