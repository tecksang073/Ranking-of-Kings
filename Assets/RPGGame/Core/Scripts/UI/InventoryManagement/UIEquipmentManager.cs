using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UIEquipmentManager : UIBase
{
    [System.Serializable]
    public struct UIEquipmentSlotContainer
    {
        public string equipPosition;
        public Transform uiContainer;
    }
    public UIItem uiEquipmentSlotPrefab;
    public UIEquipmentSlotContainer[] uiEquipmentSlotContainers;
    public UIItemList uiEquipmentList;
    public UIItemListFilterSetting filterSetting;
    public UIItem uiCharacterInfo;
    [FormerlySerializedAs("signalObjects")]
    public GameObject[] isSelectedItemSignalObjects = new GameObject[0];
    public GameObject[] notSelectedItemSignalObjects = new GameObject[0];
    public UnityEvent onIsSelectedItem = new UnityEvent();
    public UnityEvent onNotSelectedItem = new UnityEvent();
    public UnityEvent onUpdateEquipment = new UnityEvent();

    private readonly Dictionary<string, UIItem> UIEquipmentSlots = new Dictionary<string, UIItem>();
    public UIItem SelectedItem { get; private set; }

    private PlayerItem dirtyItem;
    private bool dirtyIsSelectedItem = false;

    protected virtual void Start()
    {
        foreach (var signalObject in isSelectedItemSignalObjects)
        {
            signalObject.SetActive(false);
        }
        foreach (var signalObject in notSelectedItemSignalObjects)
        {
            signalObject.SetActive(true);
        }
        onNotSelectedItem.Invoke();
    }

    protected virtual void Update()
    {
        if (dirtyItem != UIGlobalData.SelectedItem)
        {
            dirtyItem = UIGlobalData.SelectedItem;

            if (dirtyItem == null || dirtyItem.CharacterData == null)
            {
                if (uiCharacterInfo != null)
                    uiCharacterInfo.Clear();
                return;
            }

            if (uiCharacterInfo != null)
                uiCharacterInfo.SetData(dirtyItem);
        }

        bool isSelectedItem = SelectedItem != null;
        if (dirtyIsSelectedItem != isSelectedItem)
        {
            dirtyIsSelectedItem = isSelectedItem;
            foreach (var signalObject in isSelectedItemSignalObjects)
            {
                signalObject.SetActive(isSelectedItem);
            }
            foreach (var signalObject in notSelectedItemSignalObjects)
            {
                signalObject.SetActive(!isSelectedItem);
            }
            if (isSelectedItem)
                onIsSelectedItem.Invoke();
            else
                onNotSelectedItem.Invoke();
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

        if (uiCharacterInfo != null)
            uiCharacterInfo.SetData(UIGlobalData.SelectedItem);

        // Setup empty slots
        if (UIEquipmentSlots.Count == 0)
        {
            foreach (var uiEquipmentSlotContainer in uiEquipmentSlotContainers)
            {
                var equipPosition = uiEquipmentSlotContainer.equipPosition;
                if (!string.IsNullOrEmpty(equipPosition) && !UIEquipmentSlots.ContainsKey(equipPosition))
                {
                    var newEquipmentSlotObject = Instantiate(uiEquipmentSlotPrefab.gameObject);
                    newEquipmentSlotObject.transform.SetParent(uiEquipmentSlotContainer.uiContainer);
                    newEquipmentSlotObject.transform.localScale = Vector3.one;
                    newEquipmentSlotObject.SetActive(true);

                    var rectTransform = newEquipmentSlotObject.GetComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;

                    var newEquipmentSlot = newEquipmentSlotObject.GetComponent<UIItem>();
                    newEquipmentSlot.SetData(null);
                    newEquipmentSlot.notShowEquippedStatus = true;
                    newEquipmentSlot.selectionMode = UIDataItemSelectionMode.Default;
                    newEquipmentSlot.eventSelect.RemoveListener(OnClickUIEquipmentSlot);
                    newEquipmentSlot.eventSelect.AddListener(OnClickUIEquipmentSlot);

                    UIEquipmentSlots.Add(equipPosition, newEquipmentSlot);
                }
            }
        }

        var equippedItems = UIGlobalData.SelectedItem.EquippedItems;
        // Set item to slots
        foreach (var slot in UIEquipmentSlots)
        {
            var equipmentPosition = slot.Key;
            var uiItem = slot.Value;

            PlayerItem item = null;
            if (equippedItems.TryGetValue(equipmentPosition, out item))
            {
                uiItem.SetData(item);
                uiItem.SetGraphicsAlpha(1);
            }
            else
            {
                uiItem.SetData(null);
                uiItem.SetGraphicsAlpha(0);
            }
        }

        if (uiEquipmentList != null)
        {
            filterSetting.showMaterial = false;
            filterSetting.showCharacter = false;
            filterSetting.showEquipment = true;
            filterSetting.dontShowInTeamCharacter = false;
            var list = PlayerItem.DataMap.Values.Where(a => UIItemListFilter.Filter(a, filterSetting)).ToList();
            list.SortLevel();
            uiEquipmentList.selectionMode = UIDataItemSelectionMode.Toggle;
            uiEquipmentList.eventSelect.RemoveListener(SelectItem);
            uiEquipmentList.eventSelect.AddListener(SelectItem);
            uiEquipmentList.eventDeselect.RemoveListener(DeselectItem);
            uiEquipmentList.eventDeselect.AddListener(DeselectItem);
            //  TODO: Set equipment status
            uiEquipmentList.SetListItems(list, (ui) =>
            {
            });
        }
    }

    public void Clear()
    {
        if (uiCharacterInfo != null)
            uiCharacterInfo.Clear();

        if (uiEquipmentList != null)
            uiEquipmentList.ClearListItems();
    }

    private void SelectItem(UIDataItem ui)
    {
        SelectedItem = ui as UIItem;
    }

    private void DeselectItem(UIDataItem ui)
    {
        SelectedItem = null;
    }

    public void ClearSelectedItem()
    {
        if (SelectedItem != null)
            SelectedItem.Deselect();
        SelectedItem = null;
    }

    private void OnClickUIEquipmentSlot(UIDataItem ui)
    {
        var uiItem = ui as UIItem;
        var position = GetEquipmentPosition(uiItem);
        if (SelectedItem != null)
        {
            GameInstance.GameService.EquipItem(UIGlobalData.SelectedItem.Id, SelectedItem.data.Id, position, OnSetEquipmentSuccess, OnSetEquipmentFail);
            ClearSelectedItem();
        }
        else if (!uiItem.IsEmpty())
            GameInstance.GameService.UnEquipItem(uiItem.data.Id, OnSetEquipmentSuccess, OnSetEquipmentFail);
    }

    private void OnSetEquipmentSuccess(ItemResult result)
    {
        GameInstance.Singleton.OnGameServiceItemResult(result);
        Setup();
        onUpdateEquipment.Invoke();
    }

    private void OnSetEquipmentFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }

    public string GetEquipmentPosition(UIItem ui)
    {
        foreach (var uiEquipmentSlot in UIEquipmentSlots)
        {
            if (uiEquipmentSlot.Value == ui)
                return uiEquipmentSlot.Key;
        }
        return string.Empty;
    }
}
