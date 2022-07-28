using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILootBoxManager : UIBase
{
    public UILootBoxList uiLootBoxList;
    public List<string> categories;

    public override void Show()
    {
        base.Show();

        if (uiLootBoxList != null)
        {
            var availableLootBoxes = GameInstance.AvailableLootBoxes;
            var allLootBoxes = GameInstance.GameDatabase.LootBoxes;
            var list = allLootBoxes.Values.Where(a => availableLootBoxes.Contains(a.Id) && ContainCategory(a.category)).ToList();
            uiLootBoxList.SetListItems(list);
        }
    }

    public bool ContainCategory(string category)
    {
        if (categories == null || categories.Count == 0)
            return true;
        return categories.Contains(category);
    }
}
