using UnityEngine;

public abstract class ItemEffectSO : ScriptableObject
{
    public ItemSFXManager sfx;
    public abstract void UseItem();
}
