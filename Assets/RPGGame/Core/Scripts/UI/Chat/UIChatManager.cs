using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIChatManager : UIBase
{
    public float reloadDuration = 1f;
    public bool isClanChat;
    public UIChatList uiChatList;
    public InputField messageInput;
    public ScrollRect scrollRect;
    public KeyCode enterKey = KeyCode.Return;
    private float reloadCountDown = 0f;
    private long? lastTime;
    private Dictionary<string, ChatMessage> allChats = new Dictionary<string, ChatMessage>();

    public string Message { get => messageInput.text; set => messageInput.text = value; }

    public override void Show()
    {
        base.Show();
        Reload();
    }

    public override void Hide()
    {
        base.Hide();
        if (uiChatList != null)
            uiChatList.ClearListItems();
    }

    public void Reload()
    {
        if (!lastTime.HasValue)
            lastTime = GameInstance.GameService.Timestamp;
        if (isClanChat)
            GameInstance.GameService.GetClanChatMessages(lastTime.Value, OnGetChatMessages);
        else
            GameInstance.GameService.GetChatMessages(lastTime.Value, OnGetChatMessages);
    }

    private void OnGetChatMessages(ChatMessageListResult result)
    {
        foreach (var chat in result.list)
        {
            allChats[chat.Id] = chat;
        }
        var sortedChats = new List<ChatMessage>(allChats.Values);
        sortedChats.Sort();
        if (uiChatList != null)
        {
            uiChatList.selectionMode = UIDataItemSelectionMode.Disable;
            foreach (var chat in sortedChats)
            {
                var ui = uiChatList.SetListItem(chat);
                ui.uiChatManager = this;
            }
        }
        if (sortedChats.Count > 0)
        {
            var newLastTime = sortedChats.Last().ChatTime;
            if (lastTime.Value < newLastTime)
            {
                lastTime = newLastTime;
                StartCoroutine(VerticalScroll(0f));
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(enterKey))
        {
            OnClickSend();
        }
        reloadCountDown -= Time.deltaTime;
        if (reloadCountDown <= 0)
        {
            reloadCountDown = reloadDuration;
            Reload();
        }
    }

    public void OnClickSend()
    {
        if (string.IsNullOrEmpty(Message))
            return;
        messageInput.interactable = false;
        if (isClanChat)
        {
            GameInstance.GameService.EnterClanChatMessage(Message, (result) =>
            {
                Message = string.Empty;
                messageInput.interactable = true;
            }, (error) =>
            {
                Message = string.Empty;
                messageInput.interactable = true;
                Debug.LogError("[EnterClanChatMessage] " + error);
            });
        }
        else
        {
            GameInstance.GameService.EnterChatMessage(Message, (result) =>
            {
                Message = string.Empty;
                messageInput.interactable = true;
            }, (error) =>
            {
                Message = string.Empty;
                messageInput.interactable = true;
                Debug.LogError("[EnterChatMessage] " + error);
            });
        }
    }

    IEnumerator VerticalScroll(float normalize)
    {
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            yield return new WaitForSeconds(1f);
            scrollRect.verticalScrollbar.value = normalize;
            Canvas.ForceUpdateCanvases();
        }
    }
}
