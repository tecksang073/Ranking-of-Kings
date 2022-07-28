using UnityEngine;

public class AnimItemRewarding : MonoBehaviour
{
    public UIItem uiRewardItem;
    public ObjectsActivatingByItemTiers objectsActivatingByItemTiers = new ObjectsActivatingByItemTiers();

    public virtual void Play(PlayerItem item)
    {
        objectsActivatingByItemTiers.Activate(item.Tier);
        uiRewardItem.SetData(item);
    }
}
