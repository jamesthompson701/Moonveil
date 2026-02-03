using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//Contains info for dropped items and inventory items alike

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    //numerical ID and name
    public int itemID;
    public string itemName;

    //mesh renderer for when its dropped? temporary
    public MeshRenderer itemModel;

}
