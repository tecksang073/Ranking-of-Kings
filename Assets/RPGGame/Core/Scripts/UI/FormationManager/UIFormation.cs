using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class UIFormation : UIBase
{
    [System.Serializable]
    public struct ObjectsByFormationPosition
    {
        public GameObject[] objects;
        public void SetActive(bool isActive)
        {
            if (objects == null || objects.Length == 0)
                return;
            foreach (var obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }

    public string formationName;
    public Transform[] uiContainers;
    public GameObject guideObject;
    public Text textTeamBattlePoint;
    [Tooltip("These game objects will be activate when the formation has a character, index of this list is position in formation")]
    public ObjectsByFormationPosition[] notEmptySignalObjectsByFormationPositions = new ObjectsByFormationPosition[0];
    [FormerlySerializedAs("signalObjects")]
    public GameObject[] isSelectedItemSignalObjects = new GameObject[0];
    public GameObject[] notSelectedItemSignalObjects = new GameObject[0];
    public UnityEvent onIsSelectedItem = new UnityEvent();
    public UnityEvent onNotSelectedItem = new UnityEvent();
    public UnityEvent onUpdateFormation = new UnityEvent();

    private UIFormationManager manager;
    private UIItem slotPrefab;
    private readonly List<UIItem> UIFormationSlots = new List<UIItem>();
    private bool dirtyIsSelectedItem = false;

    public int TeamBattlePoint { get; private set; }

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
        bool isSelectedItem = manager != null && manager.SelectedItem != null;
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

    public void SetFormationData(UIFormationManager manager)
    {
        this.manager = manager;
        SetFormationData(manager.uiFormationSlotPrefab);
    }

    public void SetFormationData(UIItem slotPrefab)
    {
        this.slotPrefab = slotPrefab;
        foreach (var notEmptySignalObjectsByFormationPosition in notEmptySignalObjectsByFormationPositions)
        {
            notEmptySignalObjectsByFormationPosition.SetActive(false);
        }

        if (UIFormationSlots.Count == 0)
        {
            foreach (var uiContainer in uiContainers)
            {
                var newItemObject = Instantiate(slotPrefab.gameObject);
                newItemObject.transform.SetParent(uiContainer);
                newItemObject.transform.localScale = Vector3.one;
                newItemObject.SetActive(true);

                var rectTransform = newItemObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;

                var newItem = newItemObject.GetComponent<UIItem>();
                newItem.SetData(null);
                newItem.notShowInTeamStatus = true;
                newItem.selectionMode = UIDataItemSelectionMode.Default;
                newItem.eventSelect.RemoveListener(OnClickUITeamMember);
                newItem.eventSelect.AddListener(OnClickUITeamMember);
                UIFormationSlots.Add(newItem);
            }
        }
        TeamBattlePoint = 0;
        var i = 0;
        foreach (var uiItem in UIFormationSlots)
        {
            PlayerFormation playerFormation = null;
            if (PlayerFormation.TryGetData(formationName, i, out playerFormation))
            {
                var itemId = playerFormation.ItemId;
                PlayerItem item = null;
                if (!string.IsNullOrEmpty(itemId) && PlayerItem.DataMap.TryGetValue(itemId, out item))
                {
                    uiItem.SetData(item);
                    uiItem.SetGraphicsAlpha(1);
                    TeamBattlePoint += item.BattlePoint;
                    if (notEmptySignalObjectsByFormationPositions.Length > i)
                        notEmptySignalObjectsByFormationPositions[i].SetActive(true);
                }
                else
                {
                    uiItem.SetData(null);
                    uiItem.SetGraphicsAlpha(0);
                }
            }
            else
            {
                uiItem.SetData(null);
                uiItem.SetGraphicsAlpha(0);
            }
            ++i;
        }
        if (textTeamBattlePoint != null)
            textTeamBattlePoint.text = TeamBattlePoint.ToString("N0");
    }

    public void RemoveFromFormation(int position)
    {
        GameInstance.GameService.SetFormation(string.Empty, formationName, position, OnSetFormationSuccess, OnSetFormationFail);
    }

    private void OnClickUITeamMember(UIDataItem ui)
    {
        var uiItem = ui as UIItem;
        var position = GetFormationPosition(uiItem);
        if (manager != null)
        {
            if (manager.SelectedItem != null)
            {
                GameInstance.GameService.SetFormation(manager.SelectedItem.data.Id, formationName, position, OnSetFormationSuccess, OnSetFormationFail);
                manager.ClearSelectedItem();
            }
            else if (!uiItem.IsEmpty())
                RemoveFromFormation(position);
        }
    }

    private void OnSetFormationSuccess(FormationListResult result)
    {
        GameInstance.Singleton.OnGameServiceFormationListResult(result);
        SetFormationData(slotPrefab);
        onUpdateFormation.Invoke();
        manager.UpdateCharacterFormations();
    }

    private void OnSetFormationFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }

    public void ShowGuideObject()
    {
        if (guideObject != null)
            guideObject.SetActive(true);
    }

    public void HideGuideObject()
    {
        if (guideObject != null)
            guideObject.SetActive(true);
    }

    public int GetFormationPosition(UIItem ui)
    {
        return UIFormationSlots.IndexOf(ui);
    }
}
