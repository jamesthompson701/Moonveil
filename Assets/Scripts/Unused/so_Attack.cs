using System.Collections.Generic;
using UnityEngine;

public enum eSpellUpgradeLevel { basic, advanced, expert }
public enum eStatusEffect { burn, slip, push, stun, none }
public enum eSpellType { fire, water, earth, air }

/// <summary>
/// This handles spell and attack stats
/// </summary>

[CreateAssetMenu(fileName = "New Attack", menuName = "Create Attack/Create Attack")]

public class so_Attack : ScriptableObject
{
    // Common stats for spells
    public eSpellType spellType;
    public int spellSize; // area of effect size
    public eSpellUpgradeLevel spellUpgradeLevel;
    public int spellDuration; // duration of spell effect
    public bool isAttackSpell;
    public int castDelay; // delay before spell is cast

    // Common stats for non-attack spells
    // TODO determine any stats needed for non-attack spells

    // Common stats for attacks / attack spells
    public eStatusEffect statusEffect;
    public int attackPower;
    public bool appliesStatusEffect;
    public int statusDuration;
    public float statusChance; // percentage chance to apply status effect
    public int attackRange;
    public int kockbackForce;

}
