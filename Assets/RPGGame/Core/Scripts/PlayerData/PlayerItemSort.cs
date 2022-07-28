using System.Collections.Generic;

public static class PlayerItemSort
{
    public static void SortLevel(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Level.CompareTo(b.Level) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

    public static void SortSellPrice(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.SellPrice.CompareTo(b.SellPrice) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

    public static void SortRewardExp(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.RewardExp.CompareTo(b.RewardExp) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

    public static void SortHp(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.hp.CompareTo(b.Attributes.hp) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

    public static void SortPAtk(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.pAtk.CompareTo(b.Attributes.pAtk) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

    public static void SortPDef(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.pDef.CompareTo(b.Attributes.pDef) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

#if !NO_MAGIC_STATS
    public static void SortMAtk(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.mAtk.CompareTo(b.Attributes.mAtk) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

    public static void SortMDef(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.mDef.CompareTo(b.Attributes.mDef) * 100000;
            return aValue.CompareTo(bValue);
        });
    }
#endif

    public static void SortSpd(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.spd.CompareTo(b.Attributes.spd) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

#if !NO_EVADE_STATS
    public static void SortEva(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.eva.CompareTo(b.Attributes.eva) * 100000;
            return aValue.CompareTo(bValue);
        });
    }

    public static void SortAcc(this List<PlayerItem> item)
    {
        item.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a != null && b == null) return 1;
            if (a == null && b != null) return -1;
            var aValue = PlayerFormation.ContainsDataWithItemId(a.Id) || a.EquippedByItem != null ? 0 : 1000000;
            var bValue = PlayerFormation.ContainsDataWithItemId(b.Id) || b.EquippedByItem != null ? 0 : 1000000;
            aValue -= a.Attributes.acc.CompareTo(b.Attributes.acc) * 100000;
            return aValue.CompareTo(bValue);
        });
    }
#endif
}
