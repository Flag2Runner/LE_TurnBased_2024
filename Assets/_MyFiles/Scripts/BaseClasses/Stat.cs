using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat : MonoBehaviour
{
    [SerializeField] private string StatName;
    [SerializeField] private int BaseValue;
    [SerializeField] private List<int> Modifiers = new List<int>();

    public int GetBaseValue() { return BaseValue; }
    public void SetBaseValue(int value) { BaseValue = value; }
    public string GetStatName() { return StatName; }
    public void SetStatName(string nameToSet) { StatName = nameToSet; }

    public int GetValue()
    {
        int totalValue = BaseValue;
        Modifiers.ForEach(x => totalValue += x);
        return totalValue;
    }

    public void AddModifier(int modifier) { if (modifier != 0) { Modifiers.Add(modifier); } }
    public void RemoveModifier(int modifier) { if (modifier != 0) { Modifiers.Remove(modifier); } }

    // methods to set and modify the base value directly
    public void SetRawValue(int value) { BaseValue = value; }
    public void ModifyValue(int amount) { BaseValue += amount; }
}
