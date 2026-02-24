using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database SO", menuName = "Item Database")]
public class DatabaseSO : ScriptableObject
{
    public List<ItemSO> items;

    public ItemSO ReferenceItem(int _itemID)
    {
        foreach (var item in items)
        {
            if (item.itemID == _itemID)
            {
                return item;
            }
        }
        return null;
    }

}
