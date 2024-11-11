using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private string CharacterName;
    [SerializeField] private string CharacterDescription;
    [SerializeField] private List<Stat> StatList = new List<Stat>();

    private Dictionary<EStatType, int> temporaryModifiers = new Dictionary<EStatType, int>();

    private void Awake()
    {
        for (int i = 0; i < (int)EStatType.COUNT; i++)
        {
            EStatType type = (EStatType)i;
            Stat newStat = new Stat();
            if (type == EStatType.Health || type == EStatType.MaxHealth ||
                type == EStatType.Mana || type == EStatType.MaxMana)
            {
                newStat.SetBaseValue(100);
            }
            else
            {
                newStat.SetBaseValue(3);
            }
            newStat.SetStatName(type.ToString());
            StatList.Add(newStat);
        }
    }

    public int GetMaxHealth() => GetStat(EStatType.MaxHealth)?.GetValue() ?? 0;
    public int GetCurrentHealth() => GetStat(EStatType.Health)?.GetValue() ?? 0;

    public void UpdateHealth(int delta)
    {
        Stat healthStat = GetStat(EStatType.Health);
        if (healthStat != null)
        {
            healthStat.ModifyValue(delta);
            healthStat.SetRawValue(Mathf.Clamp(healthStat.GetValue(), 0, GetMaxHealth()));

            HealthComponent healthComponent = GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.ChangeHealth(delta, gameObject);
            }
        }
    }

    public Stat GetStat(EStatType statToGet)
    {
        foreach (Stat stat in StatList)
        {
            if (stat.GetStatName() == statToGet.ToString())
            {
                return stat;
            }
        }
        return null;
    }

    // New Methods to Implement GetAttackDamage, GetCurrentArmor, and RegenArmor
    public int GetAttackDamage()
    {
        return GetStat(EStatType.Attack)?.GetValue() ?? 0;
    }

    public int GetCurrentArmor()
    {
        return GetStat(EStatType.Armor)?.GetValue() ?? 0;
    }

    public void RegenArmor(int amount)
    {
        Stat armorStat = GetStat(EStatType.Armor);
        if (armorStat != null)
        {
            armorStat.ModifyValue(amount);
            armorStat.SetRawValue(Mathf.Max(0, armorStat.GetValue())); // Armor can't be negative
        }
    }

    public void AddTemporaryStatsModifier(int amount, EStatType statType)
    {
        Stat stat = GetStat(statType);
        if (stat != null)
        {
            if (!temporaryModifiers.ContainsKey(statType))
                temporaryModifiers[statType] = 0;

            temporaryModifiers[statType] += amount;
            stat.ModifyValue(amount);
        }
    }

    public void RemoveTemporaryStatsModifier(EStatType statType)
    {
        if (temporaryModifiers.ContainsKey(statType))
        {
            Stat stat = GetStat(statType);
            if (stat != null)
            {
                stat.ModifyValue(-temporaryModifiers[statType]);
                temporaryModifiers[statType] = 0;
            }
        }
    }
}

public enum EStatType { MaxHealth, Health, MaxMana, Mana, Attack, Armor, MaxArmor, COUNT }
