using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIFormationManager : UIBase
{
    [System.Serializable]
    public struct UIFormationToggleData
    {
        public UIFormationToggle toggler;
        [HideInInspector]
        public UIFormation uiFormation;
        public void Select()
        {
            toggler.IsOn = true;
        }
    }
    public EFormationType formationType;
    public UIFormation uiFormationPrefab;
    public UIFormationToggle uiFormationTogglerPrefab;
    public UIItem uiFormationSlotPrefab;
    public Transform togglerContainer;
    public Transform formationContainer;
    public UIItemList uiCharacterList;
    public UIItemListFilterSetting filterSetting;
    public BaseGamePlayFormation[] characterFormations = new BaseGamePlayFormation[0];
    private readonly Dictionary<string, UIFormationToggleData> UIFormationToggles = new Dictionary<string, UIFormationToggleData>();
    public UIFormationToggleData SelectedFormation { get; private set; }
    public UIItem SelectedItem { get; private set; }
    public string SelectedFormationName
    {
        get
        {
            if (SelectedFormation.uiFormation == null)
                return "";
            return SelectedFormation.uiFormation.formationName;
        }
    }

    public override void Show()
    {
        base.Show();
        Setup();
    }

    public override void Hide()
    {
        base.Hide();
        Clear();
    }

    public void Setup()
    {
        var playerId = Player.CurrentPlayerId;

        var firstKey = string.Empty;
        if (UIFormationToggles.Count == 0)
        {
            var formations = GameInstance.GameDatabase.Formations;
            foreach (var formation in formations)
            {
                var key = formation.Key;
                var value = formation.Value;
                if (value.formationType != formationType)
                    continue;
                if (!string.IsNullOrEmpty(key) && !UIFormationToggles.ContainsKey(key))
                {
                    var newFormationObject = Instantiate(uiFormationPrefab.gameObject);
                    newFormationObject.transform.SetParent(formationContainer);
                    newFormationObject.transform.localScale = Vector3.one;
                    newFormationObject.SetActive(true);

                    var rectTransform = newFormationObject.GetComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;

                    var newFormationTogglerObject = Instantiate(uiFormationTogglerPrefab.gameObject);
                    var newFormationToggler = newFormationTogglerObject.GetComponent<UIFormationToggle>();
                    newFormationTogglerObject.transform.SetParent(togglerContainer);
                    newFormationTogglerObject.transform.localPosition = Vector3.zero;
                    newFormationTogglerObject.transform.localScale = Vector3.one;
                    newFormationTogglerObject.SetActive(true);

                    newFormationToggler.formation = value;
                    newFormationToggler.TempToggle.onValueChanged.AddListener((isSelected) =>
                    {
                        if (isSelected)
                        {
                            var formationName = key;
                            SelectFormation(formationName);
                        }
                    });

                    var newFormation = newFormationObject.GetComponent<UIFormation>();
                    newFormation.formationName = key;
                    newFormation.SetFormationData(this);
                    var newFormationToggleData = new UIFormationToggleData();
                    newFormationToggleData.toggler = newFormationToggler;
                    newFormationToggleData.uiFormation = newFormation;
                    UIFormationToggles.Add(key, newFormationToggleData);
                    if (UIFormationToggles.Count == 1)
                        firstKey = key;
                }
            }
            // Must have at least one entry to set formation
            if (!string.IsNullOrEmpty(firstKey))
            {
                var playerSelectedFormation = Player.CurrentPlayer.SelectedFormation;
                if (string.IsNullOrEmpty(playerSelectedFormation) || !UIFormationToggles.ContainsKey(playerSelectedFormation))
                    SelectedFormation = UIFormationToggles[firstKey];
                else if (!string.IsNullOrEmpty(playerSelectedFormation))
                    SelectedFormation = UIFormationToggles[playerSelectedFormation];
                UIFormationToggles[SelectedFormationName].Select();
            }
        }

        if (uiCharacterList != null)
        {
            filterSetting.showMaterial = false;
            filterSetting.showCharacter = true;
            filterSetting.showEquipment = false;
            filterSetting.dontShowEquippedEquipment = false;
            var list = PlayerItem.DataMap.Values.Where(a => UIItemListFilter.Filter(a, filterSetting)).ToList();
            list.SortLevel();
            uiCharacterList.selectionMode = UIDataItemSelectionMode.Toggle;
            uiCharacterList.eventSelect.RemoveListener(SelectItem);
            uiCharacterList.eventSelect.AddListener(SelectItem);
            uiCharacterList.eventDeselect.RemoveListener(DeselectItem);
            uiCharacterList.eventDeselect.AddListener(DeselectItem);
            uiCharacterList.SetListItems(list, (ui) =>
            {
                ui.uiFormationManager = this;
            });
        }
    }

    public void Clear()
    {
        if (uiCharacterList != null)
            uiCharacterList.ClearListItems();
    }

    private void SelectItem(UIDataItem ui)
    {
        SelectedFormation.uiFormation.ShowGuideObject();
        SelectedItem = ui as UIItem;
    }

    private void DeselectItem(UIDataItem ui)
    {
        SelectedFormation.uiFormation.HideGuideObject();
        SelectedItem = null;
    }

    public void ClearSelectedItem()
    {
        if (SelectedItem != null)
            SelectedItem.Deselect();
        SelectedItem = null;
    }

    public void SelectFormation(string formationName)
    {
        if (!UIFormationToggles.ContainsKey(formationName))
            return;

        foreach (var uiFormationToggle in UIFormationToggles)
        {
            var toggler = uiFormationToggle.Value.toggler;
            var uiFormation = uiFormationToggle.Value.uiFormation;
            if (uiFormationToggle.Key == formationName)
            {
                uiFormation.Show();
                continue;
            }
            toggler.IsOn = false;
            uiFormation.Hide();
        }
        SelectedFormation = UIFormationToggles[formationName];
        GameInstance.GameService.SelectFormation(formationName, formationType, OnSelectFormationSuccess, OnSelectFormationFail);
    }

    private void OnSelectFormationSuccess(PlayerResult result)
    {
        Player.CurrentPlayer = result.player;
        UpdateCharacterFormations();
    }

    private void OnSelectFormationFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }

    public void UpdateCharacterFormations()
    {
        foreach (var characterFormation in characterFormations)
        {
            switch (formationType)
            {
                case EFormationType.Stage:
                    characterFormation.SetFormationCharactersForStage();
                    break;
                case EFormationType.Arena:
                    characterFormation.SetFormationCharactersForArena();
                    break;
            }
        }
    }
}
