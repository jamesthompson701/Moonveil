using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellUpgrade", menuName = "Scriptable Objects/ItemEffects/SpellUpgrade")]
public class SpellUpgrade : ItemEffectSO
{
    public int spellType;

    public override void UseItem()
    {
        GameObject player;
        player = GameObject.FindWithTag("Player");
        switch(spellType)
        {
            case 1:
                SpellManager2.Instance.waterTierUnlocked[1] = true;
                break;
            case 2:
            SpellManager2.Instance.earthTierUnlocked[1] = true;
                break;
            case 3:
                SpellManager2.Instance.airTierUnlocked[1] = true;
                break;
            case 4:
                SpellManager2.Instance.fireTierUnlocked[1] = true;
                break;
            case 5:
                ThirdPersonController.Instance.isFlightUnlocked = true;
                break;
        }
    }
 }
