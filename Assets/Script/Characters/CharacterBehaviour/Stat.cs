using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    float baseValue;
    [SerializeField]
    List<float> modifiers = new List<float>();

    public float GetValue()
    {
        float finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return finalValue;
    }

    public void AddModifier(float modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(float modifier)
    {
        modifiers.Remove(modifier);
    }
}
