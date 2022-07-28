using UnityEngine;

[System.Serializable]
public struct ItemTierAndObjectsTuple
{
    public ItemTier itemTier;
    public GameObject[] objects;

    public void ActivateObjects()
    {
        foreach (GameObject obj in objects)
            obj.SetActive(true);
    }

    public void DeactivateObjects()
    {
        foreach (GameObject obj in objects)
            obj.SetActive(false);
    }
}
