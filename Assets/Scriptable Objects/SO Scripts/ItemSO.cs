using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//Contains info for dropped items and inventory items alike

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    //Item Info
    public int itemID;
    public string itemName;
    public string itemDescription;

    public bool isStackable = true;

    //Item Arts
    public Sprite itemSprite;

    //mesh renderer for when its dropped? temporary
    public MeshRenderer itemModel;

}
