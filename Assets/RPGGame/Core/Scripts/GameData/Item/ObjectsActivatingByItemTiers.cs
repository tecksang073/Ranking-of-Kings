[System.Serializable]
public class ObjectsActivatingByItemTiers
{
    public ItemTierAndObjectsTuple[] list = new ItemTierAndObjectsTuple[0];

    public void Activate(ItemTier itemTier)
    {
        foreach (var item in list)
        {
            if (item.itemTier == itemTier)
                item.ActivateObjects();
            else
                item.DeactivateObjects();
        }
    }
}
