using System.Collections.Generic;
using UnityEngine;

public enum eWeaknessType { fire, water, earth, air, none }
public enum eStrengthType { fire, water, earth, air, none }
public enum eCreatureType { player, enemy, npc }

/// <summary>
/// This will handle all creature stats
/// </summary>

[CreateAssetMenu(fileName = "New Creature", menuName = "Create Creature/Create Creature")]

public class so_Creatures : ScriptableObject
{
    // Common stats for everything
    public string description;

    // Common stats for creatures and the player
    public eWeaknessType weaknessType;
    public eCreatureType creatureType;
    public eStrengthType strengthType;
    public string creatureName;
    public int maxHealth;
    public List<string> enemyDrops;


}
