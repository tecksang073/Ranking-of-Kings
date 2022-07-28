using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIFindClanList : UIClanList
{
    public InputField inputClanName;

    private void OnEnable()
    {
        inputClanName.text = "";
        OnClickFind();
    }

    public void OnClickFind()
    {
        GameInstance.GameService.FindClan(inputClanName.text, OnFindClanSuccess, OnFindClanFail);
    }

    private void OnFindClanSuccess(ClanListResult result)
    {
        SetListItems(result.list);
    }

    private void OnFindClanFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
