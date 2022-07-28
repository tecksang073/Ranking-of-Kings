using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInGamePackageManager : UIBase
{
    public UIInGamePackageList uiInGamePackageList;
    public List<string> categories;

    public override void Show()
    {
        base.Show();

        if (uiInGamePackageList != null)
        {
            var availableInGamePackages = GameInstance.AvailableInGamePackages;
            var allInGamePackagees = GameInstance.GameDatabase.InGamePackages;
            var list = allInGamePackagees.Values.Where(a => availableInGamePackages.Contains(a.Id) && ContainCategory(a.category)).ToList();
            uiInGamePackageList.SetListItems(list);
        }
    }

    public bool ContainCategory(string category)
    {
        if (categories == null || categories.Count == 0)
            return true;
        return categories.Contains(category);
    }
}
