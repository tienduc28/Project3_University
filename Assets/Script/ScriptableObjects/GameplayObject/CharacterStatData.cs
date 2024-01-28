using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatData", menuName = "Combat/StatData")]
public class CharacterStatData : ScriptableObject
{
    public Stat maxHealth;
    public Stat armor;
    public Stat damage;
    public Stat attackSpeed;
    public Stat attackRange;
    public Stat visionRange;
    public Stat maxStamina;

    public Stat attackKnockbackDistance;
    public Stat knockbackResistance;
}
