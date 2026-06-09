using UnityEngine;

[CreateAssetMenu(fileName = "InventoryRepository", menuName = "Scriptable Objects/InventoryRepository")]
public class InventoryRepository : ScriptableObject
{
    public ItemSO[] items;
}
